//using System;
//using System.Security.Cryptography.X509Certificates;
//using VRT.Xml.Xades.StaticSets;

//namespace VRT.Xml.Xades.Services
//{
//    public class CertyficateService: ICertyficateService
//    {
//        public bool CardCertyficatesOnly { get; set; }
//        public bool OnlyWithPrivateKeys { get; set; }
//        public string DialogTitle { get; set; }
//        public string DialogMessage { get; set; }
//        public bool QualifiedOnly { get; set; }

//        public X509Certificate2 GetCertyficate()
//        {
//            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);

//            try
//            {
//                store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

//                var collection = store.Certificates;
//                var fcollection = collection
//                    .Find(X509FindType.FindByTimeValid, DateTime.Now, false);

//                if (CardCertyficatesOnly)
//                {
//                    fcollection = fcollection
//                        .Find(X509FindType.FindByExtension, new X509KeyUsageExtension().Oid.Value, false)
//                        .Find(X509FindType.FindByKeyUsage, X509KeyUsageFlags.NonRepudiation, false);
//                }

//                var finalCollection = new X509Certificate2Collection();
//                foreach (var tmpCert in fcollection)
//                {
//                    if (OnlyWithPrivateKeys && !tmpCert.HasPrivateKey)
//                        continue;
//                    if(QualifiedOnly && !Qca.IsQca(tmpCert))
//                        continue;
//                    finalCollection.Add(tmpCert);
//                }

//                if (string.IsNullOrEmpty(DialogMessage))
//                {
//                    DialogMessage = "Wybierz certyfikat";
//                }

//                if (string.IsNullOrEmpty(DialogTitle))
//                {
//                    DialogTitle = "Podpisywanie danych";
//                }
//                if (finalCollection.Count == 1)
//                    return finalCollection[0];

//                var scollection = X509Certificate2UI
//                    .SelectFromCollection(finalCollection, DialogTitle, DialogMessage,
//                        X509SelectionFlag.SingleSelection);

//                return scollection.Count == 1 
//                    ? scollection[0] 
//                    : null;
//            }
//            catch (Exception ex)
//            {
//                throw new Exception("Nieoczekiwany wyjątek", ex);
//            }
//            finally
//            {
//                store.Close();
//            }
//        }
//    }
//}
