using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;

namespace API_WebH3.Service;

public class S3Service
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;

    public S3Service(IConfiguration config)
    {
        var awsOptions = config.GetSection("AWS");
        var credentials = new Amazon.Runtime.BasicAWSCredentials(
            awsOptions["AccessKey"],
            awsOptions["SecretKey"]);

        _s3Client = new AmazonS3Client(credentials, RegionEndpoint.GetBySystemName(awsOptions["Region"]));
        _bucketName = awsOptions["BucketName"];
    }

    public async Task<string> UploadVideoAsync(IFormFile file)
    {
        var fileTransferUtility = new TransferUtility(_s3Client);
        
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var key = $"videos/{fileName}";

        using (var stream = file.OpenReadStream())
        {
            await fileTransferUtility.UploadAsync(stream, _bucketName, key);
        }

        return fileName;
    }
    
    public async Task<Stream> GetVideoStreamAsync(string fileName)
    {
        var key = $"videos/{fileName}";

        var response = await _s3Client.GetObjectAsync(_bucketName, key);
        return response.ResponseStream;
    }

}