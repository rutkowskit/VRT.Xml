using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using Microsoft.Xades;
using VRT.Xml.Xades.Transforms;

namespace VRT.Xml.Xades.Extensions
{
    public static class XadesSignedXmlExtensions
    {
        internal static XadesSignedXml DodajReferencje(this XadesSignedXml xadesXml, ReferenceLoader references)
        {
            if (null == xadesXml) return null;
            foreach (var @ref in references.SignatureReferences)
            {
                if (string.IsNullOrWhiteSpace(@ref.Uri))
                {
                    xadesXml.AddSignatureSelfReference(@ref);
                }
                else
                {
                    xadesXml.AddSignedFileReference(@ref);
                }
            }
            return xadesXml;
        }

        private static void AddSignatureSelfReference(this XadesSignedXml sigXml, Reference template)
        {
            new XPathTransformBuilder<XadesSignedXml>(sigXml, t =>
                {
                    var reference = new Reference("")
                    {
                        Id = template.Id,
                        DigestMethod = template.DigestMethod
                    };
                    reference.AddTransform(t);
                    sigXml.AddReference(reference);
                })
                .AddNamespace("xmlns:xades", XadesSignedXml.XadesNamespaceUri)
                .AddNamespace("xmlns:ds", SignedXml.XmlDsigNamespaceUrl)
                .WithXpath("not(ancestor-or-self::ds:Signature)")
                .Done();
        }
        private static void AddSignedFileReference(this SignedXml sigXml, Reference template)
        {
            new XPathTransformBuilder<SignedXml>(sigXml, t =>
                {
                    var reference = new Reference(template.Uri)
                    {
                        Id = template.Id,
                        DigestMethod = template.DigestMethod,
                        DigestValue = template.DigestValue
                    };
                    reference.AddTransform(t);
                    sigXml.AddReference(reference);
                })
                .AddNamespace("xmlns:xades", XadesSignedXml.XadesNamespaceUri)
                .AddNamespace("xmlns:ds", SignedXml.XmlDsigNamespaceUrl)
                .WithXpath("not(ancestor-or-self::*[contains(@Id,\"pid-8ae3bccf-e44c-4707-bc4d-76e46fbe1aae\") and ancestor-or-self::xades:UnsignedProperties])")
                .Done();
        }

        public static XadesSignedXml AddKeyInfo(this XadesSignedXml signedXml, X509Certificate cert)
        {
            if (null == signedXml) return null;
            if (null == cert)
                throw new NullReferenceException("Brak ustawionego certyfikatu");

            var keyInfo = new KeyInfo();
            keyInfo.AddClause(new KeyInfoX509Data(cert));
            signedXml.KeyInfo = keyInfo;
            return signedXml;
        }

        public static XadesSignedXml SetSigningCertyficate(this XadesSignedXml signedXml, string signatureId, X509Certificate2 cert)
        {
            if (null == signedXml) return null;
            cert .AssertCert();

            var rsaKey = (RSACryptoServiceProvider)cert.PrivateKey;
            signedXml.Signature.Id = signatureId;
            signedXml.SigningKey = rsaKey;
            signedXml.SignedInfo.Id = $"ID-{Guid.NewGuid()}";
            signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigC14NTransformUrl;
            signedXml.SignedInfo.SignatureMethod = SignedXml.XmlDsigRSASHA1Url;
            return signedXml;
        }

        private static CommitmentTypeIndicationCollection GetProofOfApproval()
        {
            return new CommitmentTypeIndicationCollection
            {
                new CommitmentTypeIndication()
                {
                    AllSignedDataObjects = true,
                    CommitmentTypeId = new ObjectIdentifier("CommitmentTypeId")
                    {
                        Identifier = new Identifier()
                        {
                            IdentifierUri = @"http://uri.etsi.org/01903/v1.2.2#ProofOfApproval"
                        }
                    }
                }
            };
        }

        public static XadesSignedXml AddXadesObject(this XadesSignedXml signedXml, X509Certificate2 cert)
        {
            if (null == signedXml) return null;
            cert.AssertCert();

            var xadesObject = signedXml
                .ToXadexObject()
                .AddCertificate(cert);

            signedXml.AddXadesObject(xadesObject);

            return signedXml;
        }

        private static XadesObject ToXadexObject(this SignedXml signedXml)
        {
            if (null == signedXml) return null;
            return new XadesObject
            {
                QualifyingProperties =
                {
                    Id = $"ID-{Guid.NewGuid().ToString()}",
                    Target = $"#{signedXml.Signature.Id}",
                    SignedProperties =
                    {
                        Id = $"ID-xades-{signedXml.Signature.Id}",
                        SignedSignatureProperties =
                        {
                            SigningTime = DateTime.Now,
                            SignaturePolicyIdentifier = {SignaturePolicyImplied = true}
                        },
                        SignedDataObjectProperties = new SignedDataObjectProperties()
                        {
                            CommitmentTypeIndicationCollection = GetProofOfApproval()
                        }
                    }
                }
            };
        }
        private static void AssertCert(this X509Certificate2 cert)
        {
            if (null == cert) throw new NullReferenceException("Brak ustawionego certyfikatu");
            if (cert.HasPrivateKey == false) throw new NullReferenceException("Certyfikat nie zawiera klucza prywatnego");
        }
    }
}
