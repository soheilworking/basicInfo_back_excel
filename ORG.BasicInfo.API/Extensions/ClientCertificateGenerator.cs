using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
namespace ORG.BasicInfo.API.Extensions
{


    public static class ClientCertificateGenerator
    {
        public static byte[] CreateClientCertificate(string userName, string pfxPassword)
        {
            var rootCA = new X509Certificate2(
                "./certificate/rootCA.pfx",
                "361SoheiL@",
               X509KeyStorageFlags.Exportable | X509KeyStorageFlags.EphemeralKeySet);

            using var ecdsa = ECDsa.Create(ECCurve.NamedCurves.brainpoolP384r1);

            var req = new CertificateRequest(
                $"CN={userName}",
                ecdsa,
                HashAlgorithmName.SHA384);

            req.CertificateExtensions.Add(
                new X509EnhancedKeyUsageExtension(
                    new OidCollection { new Oid("1.3.6.1.5.5.7.3.2") },
                    false));

            req.CertificateExtensions.Add(
                new X509KeyUsageExtension(
                    X509KeyUsageFlags.DigitalSignature |
                    X509KeyUsageFlags.KeyEncipherment |
                    X509KeyUsageFlags.KeyAgreement,
                    false));

            req.CertificateExtensions.Add(
                new X509BasicConstraintsExtension(false, false, 0, false));

            var san = new SubjectAlternativeNameBuilder();
            san.AddUserPrincipalName(userName);
            san.AddDnsName("localhost");
            req.CertificateExtensions.Add(san.Build());

            var notBefore = rootCA.NotBefore.AddMinutes(2);
            var notAfter = rootCA.NotAfter.AddMinutes(-2);

            var clientCert = req.Create(
                rootCA,
                notBefore,
                notAfter,
                Guid.NewGuid().ToByteArray());

            var certWithKey = clientCert.CopyWithPrivateKey(ecdsa);

            // Chain کامل
            var collection = new X509Certificate2Collection();
            collection.Add(certWithKey);
            collection.Add(new X509Certificate2("./certificate/rootCA.crt"));

            return collection.Export(X509ContentType.Pfx, "");
        }

    }

}
