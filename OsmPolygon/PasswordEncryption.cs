
namespace OsmPolygon
{


    public class AesEncryptionService
    {
        
        protected string m_key;
        protected string m_iv;

        protected System.Text.Encoding m_encoding;


        public AesEncryptionService(string key, string iv, System.Text.Encoding encoding)
        {
            this.m_key = key;
            this.m_iv = iv;
            this.m_encoding = encoding;
        } // End Constructor 


        public AesEncryptionService()
            :this("1b55ec1d96f637aa7b73c31765a12c2c8fb8b9f6ae8b14396475a20ed1a83dac"
                 , "d4e3381cdd39ddb70f85e96d11b667e5"
                 , System.Text.Encoding.UTF8
        )
        { } // End Constructor 


        public string GenerateKey()
        {
            string retValue = null;

            using (System.Security.Cryptography.Aes aes = System.Security.Cryptography.Aes.Create())
            {
                aes.GenerateKey();
                aes.GenerateIV();

                byte[] bIV = aes.IV;
                byte[] bKey = aes.Key;
                aes.Clear();

                retValue = "IV: " + ByteArrayToHexString(bIV) 
                    + System.Environment.NewLine 
                    + "Key: " + ByteArrayToHexString(bKey);

                System.Array.Clear(bIV, 0, bIV.Length);
                System.Array.Clear(bKey, 0, bKey.Length);
                bIV = null;
                bKey = null;
            } // End Using aes 

            return retValue;
        } // End Function GenerateKey
        

        public string Encrypt(string plainText)
        {
            string retValue = null;

            using (System.Security.Cryptography.Aes aes = System.Security.Cryptography.Aes.Create())
            {
                byte[] baCipherTextBuffer = null;
                byte[] baPlainTextBuffer = null;
                byte[] baEncryptionKey =  HexStringToByteArray(this.m_key);
                byte[] baInitializationVector = HexStringToByteArray(this.m_iv);


                aes.Key = baEncryptionKey;
                aes.IV = baInitializationVector;


                //Get an encryptor.
                using (System.Security.Cryptography.ICryptoTransform transform = 
                    aes.CreateEncryptor(baEncryptionKey, baInitializationVector))
                {
                    //Encrypt the data.
                    using (System.IO.MemoryStream msEncrypt = new System.IO.MemoryStream())
                    {
                        using (System.Security.Cryptography.CryptoStream csEncrypt =
                            new System.Security.Cryptography.CryptoStream(
                                  msEncrypt
                                , transform
                                , System.Security.Cryptography.CryptoStreamMode.Write
                            )
                        )
                        {
                            //Convert the data to a byte array.
                            baPlainTextBuffer = this.m_encoding.GetBytes(plainText);

                            //Write all data to the crypto stream and flush it.
                            csEncrypt.Write(baPlainTextBuffer, 0, baPlainTextBuffer.Length);
                            csEncrypt.FlushFinalBlock();

                            //Get encrypted array of bytes.
                            baCipherTextBuffer = msEncrypt.ToArray();
                            csEncrypt.Clear();
                        } // End Using csEncrypt 

                    } // End Using msEncrypt 

                } // End Using transform 


                retValue = ByteArrayToHexString(baCipherTextBuffer);

                System.Array.Clear(baCipherTextBuffer, 0, baCipherTextBuffer.Length);
                System.Array.Clear(baPlainTextBuffer, 0, baPlainTextBuffer.Length);
                System.Array.Clear(baEncryptionKey, 0, baEncryptionKey.Length);
                System.Array.Clear(baInitializationVector, 0, baInitializationVector.Length);

                baCipherTextBuffer = null;
                baPlainTextBuffer = null;
                baEncryptionKey = null;
                baInitializationVector = null;
            } // End Using aes 

            return retValue; 
        } // End Function Encrypt


        public string DeCrypt(string encryptedInput)
        {
            string returnValue = null;

            if (string.IsNullOrEmpty(encryptedInput))
            {
                throw new System.ArgumentNullException("encryptedInput", "encryptedInput may not be string.Empty or NULL, because these are invid values.");
            }

            using (System.Security.Cryptography.Aes objRijndael = System.Security.Cryptography.Aes.Create())
            {
                byte[] baCipherTextBuffer = HexStringToByteArray(encryptedInput);
                byte[] baDecryptionKey = HexStringToByteArray(this.m_key);
                byte[] baInitializationVector = HexStringToByteArray(this.m_iv);

                // This is where the message would be transmitted to a recipient
                // who already knows your secret key. Optionally, you can
                // also encrypt your secret key using a public key algorithm
                // and pass it to the mesage recipient along with the RijnDael
                // encrypted message.            
                //Get a decryptor that uses the same key and IV as the encryptor.
                using (System.Security.Cryptography.ICryptoTransform transform = objRijndael.CreateDecryptor(baDecryptionKey, baInitializationVector))
                {

                    //Now decrypt the previously encrypted message using the decryptor
                    // obtained in the above step.
                    using (System.IO.MemoryStream msDecrypt = new System.IO.MemoryStream(baCipherTextBuffer))
                    {
                        using (System.Security.Cryptography.CryptoStream csDecrypt =
                            new System.Security.Cryptography.CryptoStream(
                              msDecrypt
                            , transform
                            , System.Security.Cryptography.CryptoStreamMode.Read)
                        )
                        {
                            byte[] baPlainTextBuffer = new byte[baCipherTextBuffer.Length];
                            
                            //Read the data out of the crypto stream.
                            csDecrypt.Read(baPlainTextBuffer, 0, baPlainTextBuffer.Length);

                            //Convert the byte array back into a string.
                            returnValue = this.m_encoding.GetString(baPlainTextBuffer);
                            System.Array.Clear(baPlainTextBuffer, 0, baPlainTextBuffer.Length);
                            baPlainTextBuffer = null;

                            if (!string.IsNullOrEmpty(returnValue))
                                returnValue = returnValue.Trim('\0');

                            csDecrypt.Clear();
                        } // End Using csDecrypt 

                    } // End Using msDecrypt 

                } // End Using transform 
                
            } // End Using aes 

            return returnValue;
        } // End Function DeCrypt


        // VB.NET to convert a byte array into a hex string
        public static string ByteArrayToHexString(byte[] arrInput)
        {
            System.Text.StringBuilder strOutput = new System.Text.StringBuilder(arrInput.Length);

            for (int i = 0; i <= arrInput.Length - 1; i++)
            {
                strOutput.Append(arrInput[i].ToString("X2"));
            }

            return strOutput.ToString().ToLower();
        } // End Function ByteArrayToHexString


        public static byte[] HexStringToByteArray(string strHexString)
        {
            int iNumberOfChars = strHexString.Length;
            byte[] baBuffer = new byte[iNumberOfChars / 2];
            for (int i = 0; i <= iNumberOfChars - 1; i += 2)
            {
                baBuffer[i / 2] = System.Convert.ToByte(strHexString.Substring(i, 2), 16);
            }
            return baBuffer;
        } // End Function HexStringToByteArray

    } // End Class AES 



    public class UserHash
    {
        protected System.Text.Encoding m_encoding;


        public UserHash(System.Text.Encoding encoding)
        {
            this.m_encoding = encoding;
        }

        public UserHash()
            : this(System.Text.Encoding.ASCII)
        { }


        public string HashCorUser(string userName)
        {
            string prexif = "IsCorUser";
            return MD5_Hash(prexif + userName.ToLowerInvariant()).ToLowerInvariant();
        }


        public string HashUser(string userName)
        {
            userName = userName.ToLowerInvariant();
            return MD5_Hash(userName);
        }


        protected string MD5_Hash(string inputString)
        {
            string hash = "";

            byte[] bytes = this.m_encoding.GetBytes(inputString);

            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(bytes);
                for (int i = 0; i < hashBytes.Length; ++i)
                {
                    hash += hashBytes[i].ToString("X2");
                } // Next i 

                System.Array.Clear(bytes, 0, bytes.Length);
                bytes = null;

                System.Array.Clear(hashBytes, 0, hashBytes.Length);
                hashBytes = null;
            } // End Using md5 

            return hash;
        } // End Function MD5_Hash 

    } // End Class UserHash 


    public class Des3EncryptionService
    {
        protected string m_symmetricKey;
        protected System.Text.Encoding m_encoding;


        public Des3EncryptionService(string symmetricKey, System.Text.Encoding encoding)
        {
            this.m_symmetricKey = symmetricKey;
            // this.m_symmetricKey = "Als symmetrischer Key kann irgendein Text verwendet werden. äöü'";
            this.m_encoding = encoding;
        } // End Constructor 


        public Des3EncryptionService()
            :this("z67f3GHhdga78g3gZUIT(6/&ns289hsB_5Tzu6", System.Text.Encoding.ASCII)
        { } // End Constructor 


        // http://www.codeproject.com/KB/aspnet/ASPNET_20_Webconfig.aspx
        // http://www.codeproject.com/KB/database/Connection_Strings.aspx
        public string DeCrypt(string strSourceText)
        {
            string result = null;

            try
            {
                using (System.Security.Cryptography.TripleDES des3 = System.Security.Cryptography.TripleDES.Create())
                {
                    using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
                    {
                        des3.Key = md5.ComputeHash(this.m_encoding.GetBytes(this.m_symmetricKey));
                        des3.Mode = System.Security.Cryptography.CipherMode.ECB;
                    } // End Using md5 

                    if (Microsoft.VisualBasic.CompilerServices.Operators.CompareString(strSourceText, "", false) != 0)
                    {
                        using (System.Security.Cryptography.ICryptoTransform cryptoTransform = des3.CreateDecryptor())
                        {
                            byte[] array = System.Convert.FromBase64String(strSourceText);
                            des3.Clear();
                            result = this.m_encoding.GetString(cryptoTransform.TransformFinalBlock(array, 0, array.Length));
                            System.Array.Clear(array, 0, array.Length);
                            array = null;
                        } // End Using cryptoTransform 
                    }
                    else
                    {
                        result = "";
                    }
                } // End Using des3 

                return result;
            } // End Try 
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                // TODO: Log 
                throw;
            }
        }


        public string Crypt(string strSourceText)
        {
            string result = null;
            try
            {
                using (System.Security.Cryptography.TripleDES des3 = System.Security.Cryptography.TripleDES.Create())
                {
                    using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
                    {
                        des3.Key = md5.ComputeHash(this.m_encoding.GetBytes(this.m_symmetricKey));
                    } // End Using md5 

                    des3.Mode = System.Security.Cryptography.CipherMode.ECB;
                    using (System.Security.Cryptography.ICryptoTransform cryptoTransform = des3.CreateEncryptor())
                    {
                        byte[] bytes = this.m_encoding.GetBytes(strSourceText);
                        result = System.Convert.ToBase64String(cryptoTransform.TransformFinalBlock(bytes, 0, bytes.Length));
                        System.Array.Clear(bytes, 0, bytes.Length);
                        bytes = null;
                    } // End Using cryptoTransform 
                } // End Using des3 

                return result;
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                // TODO: Log 
                throw;
            }
        }


        public static string GenerateKey()
        {
            string retValue = null;

            using (System.Security.Cryptography.TripleDES des3 = System.Security.Cryptography.TripleDES.Create())
            {
                des3.GenerateKey();
                des3.GenerateIV();

                byte[] bIV = des3.IV;
                byte[] bKey = des3.Key;
                
                retValue = "IV: " + AesEncryptionService.ByteArrayToHexString(bIV) 
                    + System.Environment.NewLine 
                    + "Key: " + AesEncryptionService.ByteArrayToHexString(bKey);
            } // End Using des3 

            return retValue;
        } // End Function GenerateKey
        

    } // End Class DES


} // End Namespace COR.Tools.Cryptography
