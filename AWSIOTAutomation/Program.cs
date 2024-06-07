using Amazon;
using Amazon.IoT;
using Amazon.IoT.Model;
using Amazon.Runtime.CredentialManagement;

public class Program
{
    public static async Task Main(string[] args)
    {
        var thingName = "AutomatedThing2";
        var policyName = "TrialHeatPumpPolicy";

        var sharedFile = new SharedCredentialsFile();
        CredentialProfile profile = sharedFile.ListProfiles().First();

        var client = new AmazonIoTClient(profile.Options.AccessKey, profile.Options.SecretKey, RegionEndpoint.EUWest1);

        var request = new CreateThingRequest
        {
            ThingName = thingName
        };

        // Create IOT Thing
        var createThingResponse = await client.CreateThingAsync(request);

        var createKeysAndCertificateRequest = new CreateKeysAndCertificateRequest
        {
            SetAsActive = true
        };

        // Create Certificate
        var createCertificateResponse = await client.CreateKeysAndCertificateAsync(createKeysAndCertificateRequest);

        //File.WriteAllText("certificate.pem.crt", createCertificateResponse.CertificatePem);
        //File.WriteAllText("public.pem.key", createCertificateResponse.KeyPair.PublicKey);
        //File.WriteAllText("private.pem.key", createCertificateResponse.KeyPair.PrivateKey);

        var attachPolicyRequest = new AttachPolicyRequest
        {
            PolicyName = policyName,
            Target = createCertificateResponse.CertificateArn
        };

        // Attach policy to certificate
        await client.AttachPolicyAsync(attachPolicyRequest);

        var attachThingPrincipalRequest = new AttachThingPrincipalRequest
        {
            ThingName = createThingResponse.ThingName,
            Principal = createCertificateResponse.CertificateArn
        };

        // Associate certificate with Thing
        await client.AttachThingPrincipalAsync(attachThingPrincipalRequest);

    }
}