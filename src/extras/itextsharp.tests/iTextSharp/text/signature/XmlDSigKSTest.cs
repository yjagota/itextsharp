﻿using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;

namespace itextsharp.tests.resources.text.signature
{
    [TestFixture]
    public class XmlDSigKSTest : XmlDSigTest
    {
        public const String KEYSTORE = @"..\..\resources\text\pdf\signature\rsa-ks\pkcs12";
        public const string PASSWORD = "password";
        public const String Src = @"..\..\resources\text\pdf\signature\xfa.pdf";
        public const String CmpDir = @"..\..\resources\text\pdf\signature\rsa-ks\";
        public const String DestDir = @"signatures\rsa-ks\";


    [Test]
    public void XmlDSigRsaKS()
    {
        Directory.CreateDirectory(DestDir);

        String filename = "xfa.signed.pdf";
        String output = DestDir + filename;

        Pkcs12Store store = new Pkcs12Store(new FileStream(KEYSTORE, FileMode.Open), PASSWORD.ToCharArray());
        String alias = "";
        List<X509Certificate> chain = new List<X509Certificate>();
        // searching for private key

        foreach(string al in store.Aliases) {
            if(store.IsKeyEntry(al) && store.GetKey(al).Key.IsPrivate) {
                alias = al;
                break;
            }
        }

        AsymmetricKeyEntry pk = store.GetKey(alias);
        foreach(X509CertificateEntry c in store.GetCertificateChain(alias))
            chain.Add(c.Certificate);

        RsaPrivateCrtKeyParameters parameters = pk.Key as RsaPrivateCrtKeyParameters;
        SignWithCertificate(Src, output, parameters, chain.ToArray(), DigestAlgorithms.SHA1);

        String cmp = SaveXmlFromResult(output);

        Assert.IsTrue(CompareXmls(cmp, CmpDir + filename.Replace(".pdf", ".xml")));
    }

    [Test]
    public void XmlDSigRsaKSPackage() {

        Directory.CreateDirectory(DestDir);

        String filename = "xfa.signed.package.pdf";
        String output = DestDir + filename;

        Pkcs12Store store = new Pkcs12Store(new FileStream(KEYSTORE, FileMode.Open), PASSWORD.ToCharArray());
        String alias = "";
        List<X509Certificate> chain = new List<X509Certificate>();
        // searching for private key

        foreach(string al in store.Aliases) {
            if(store.IsKeyEntry(al) && store.GetKey(al).Key.IsPrivate) {
                alias = al;
                break;
            }
        }

        AsymmetricKeyEntry pk = store.GetKey(alias);
        foreach(X509CertificateEntry c in store.GetCertificateChain(alias)) {
            chain.Add(c.Certificate);
        }

        RsaPrivateCrtKeyParameters parameters = pk.Key as RsaPrivateCrtKeyParameters;

        SignPackageWithCertificate(Src, output, XfaXpathConstructor.XdpPackage.Template, parameters, chain.ToArray(), DigestAlgorithms.SHA1);

        String cmp = SaveXmlFromResult(output);

        Assert.IsTrue(CompareXmls(cmp, CmpDir + filename.Replace(".pdf", ".xml")));
    }
 
    }
}