using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Amazon;
using Amazon.S3;
using Amazon.CloudFront;
using Amazon.S3.Transfer;
using SixLabors.ImageSharp.Formats.Jpeg;
using Flatlinq.Data;
using Flatlinq.Models.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Flatlinq.Services;

public class HouseServices: IHouseServices
{
    private readonly CoreDbContext _coreDbContext;
    private readonly IConfiguration _configuration;
    private readonly IJwtServices _jwtServices;
    private readonly IMemoryCache _memoryCache;
    private readonly UserManager<User> _userManager;
    public HouseServices(CoreDbContext coreDbContext, IConfiguration configuration, 
        IJwtServices jwtServices, IMemoryCache memoryCache, UserManager<User> userManager)
    {
        _coreDbContext = coreDbContext;
        _configuration = configuration;
        _jwtServices = jwtServices;
        _memoryCache = memoryCache;
        _userManager = userManager;
    }
    private static void UploadToS3(TransferUtility utility, MemoryStream stream, 
        string key, IConfiguration _configuration)
    {
        TransferUtilityUploadRequest request = new()
        {
            Key = key,
            InputStream = stream,
            BucketName = _configuration["Aws:BucketName"]!
        };
        utility.Upload(request);
    }
    private static async Task<string> SaveImage(IFormFile file, RegionEndpoint endpoint, IConfiguration _configuration)
    {
        //Get original file as stream
        using MemoryStream original = new();
        await file.CopyToAsync(original);
        original.Seek(0, SeekOrigin.Begin);

        //Blur image and save in another stream
        Image image = await Image.LoadAsync(original);
        image.Mutate(ctx => ctx.GaussianBlur(25));
        using MemoryStream blurred = new();
        await image.SaveAsJpegAsync(blurred, new JpegEncoder());
        blurred.Position = 0;

        IAmazonS3 client = new AmazonS3Client(
            _configuration["Aws:AccessKey"], _configuration["Aws:SecretAccessKey"],
            new AmazonS3Config()
            {
                RegionEndpoint = endpoint
            }
        );
        TransferUtility utility = new();
        string name = Guid.NewGuid().ToString();
        UploadToS3(utility, original, name, _configuration);
        UploadToS3(utility, blurred, string.Format("Blurred-{0}", name), _configuration);
        return name;
    }

    public async Task CreateHouse(CreateHouseDTO data, string accessToken)
    {
        string userId = _jwtServices.GetIdFromToken(accessToken);
        Landlord landlord = _coreDbContext.Landlords!.FirstOrDefault(x => x.UserId == userId)!;
        List<Task<string>> tasks = new();
        foreach (IFormFile file in data.Files)
        {
            tasks.Add(SaveImage(file, RegionEndpoint.USEast1, _configuration));
        }
        string[] imageNames = await Task.WhenAll(tasks);
        await _coreDbContext.Houses!.AddAsync(new House
        {
            Name = data.Name,
            Description = data.Description,
            HasInternet = data.HasInternet,
            HasElectricity = data.HasElectricity,
            AllowChildren = data.AllowChildren,
            AllowPets = data.AllowPets,
            AllowSmoking = data.AllowSmoking,
            Images = JsonSerializer.Serialize(imageNames),
            Landlord = landlord
        });
        await _coreDbContext.SaveChangesAsync();
    }

    public async Task<GetHouseResponseDTO> GetHouseById(string accessToken, int houseId)
    {
        string userId = _jwtServices.GetIdFromToken(accessToken);
        User? user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            House? house = await _coreDbContext.Houses!.Where(x => x.Id == houseId).FirstOrDefaultAsync();
            if (house != null)
            {
                string[] imageNames = JsonSerializer.Deserialize<string[]>(house.Images)!;
                List<Task<string>> tasks = new();
                foreach (string name in imageNames)
                {
                    tasks.Add(GetImageLink(name, !user.IsGoldMember));
                }
                string[] imageLinks = await Task.WhenAll(tasks);
                return new GetHouseResponseDTO
                {
                    Name = house.Name,
                    Description = house.Description,
                    Price = house.Price,
                    HasElectricity = house.HasElectricity,
                    HasInternet = house.HasInternet,
                    AllowChildren = house.AllowChildren,
                    AllowPets = house.AllowPets,
                    AllowSmoking = house.AllowSmoking,
                    Images = imageLinks
                };
            }
            throw new MyException("No such house");
        }
        throw new MyException("No such tenant");
    }

    public async Task<string> GetImageLink(string name, bool isBlurred)
    {
        return await Task.Run(() =>
        {
            if (isBlurred)
            {
                name = string.Format("Blurred-{0}", name);
            }
            string? link = _memoryCache.Get<string>(name);
            if (link != null)
            {
                return link;
            }
            string rootDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string privateKeyLocation = Path.Combine(rootDirectory, "private-key.pem");
            using StreamReader privateKey = File.OpenText(privateKeyLocation);
            link = AmazonCloudFrontUrlSigner.GetCustomSignedURL(
                protocol: AmazonCloudFrontUrlSigner.Protocol.https,
                distributionDomain: _configuration["Aws:DistributedDomain"]!,
                privateKey: privateKey,
                resourcePath: name,
                keyPairId: _configuration["Aws:KeyPairId"]!,
                expiresOn: DateTime.Now.Add(TimeSpan.FromDays(1)),
                activeFrom :DateTime.Now,
                ipRange: "0.0.0.0/0"
            );
            return link;
        });
    }
}