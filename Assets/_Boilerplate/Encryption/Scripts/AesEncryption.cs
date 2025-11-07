using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Security.Cryptography;
using System.IO;
using System;

namespace U9.Encryption
{
    public class AesEncryption : BaseEncryptionService
    {
		[SerializeField] private string _aesKey = "979693eb824a4713936ae035336bd169";
		[SerializeField] private CipherMode _cipherMode = CipherMode.CBC;
		[SerializeField] private PaddingMode _packingMode = PaddingMode.PKCS7;
		[SerializeField] private int _keySize = 256;
		[SerializeField] private int _blockSize = 128;

        private void Reset()
        {
			GenerateKey();
        }

        [ContextMenu("Create Key")]
		private void GenerateKey()
        {
			using (Aes aes = CreateAes())
			{
				aes.GenerateKey();
				_aesKey = Convert.ToBase64String(aes.Key);
			}
        }

		private Aes CreateAes()
        {
			Aes aes = Aes.Create();
			aes.Mode = _cipherMode;
			aes.KeySize = _keySize;
			aes.BlockSize = _blockSize;
			aes.Padding = _packingMode;
			return aes;
		}

		public override string Encrypt(string text)
		{
			byte[] array;
			byte[] iv = new byte[16];

			using (Aes aes = Aes.Create())
			{
				aes.Key =Convert.FromBase64String(_aesKey);
				aes.IV = iv;

				ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
					{
						using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
						{
							streamWriter.Write(text);
						}

						array = memoryStream.ToArray();
					}
				}
			}

			return Convert.ToBase64String(array);
		}

		public override string Decrypt(string text)
		{
			byte[] buffer = Convert.FromBase64String(text);
			byte[] iv = new byte[16];

			using (Aes aes = Aes.Create())
			{
				aes.Key = Convert.FromBase64String(_aesKey);
				aes.IV = iv;
				ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

				using (MemoryStream memoryStream = new MemoryStream(buffer))
				{
					using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
					{
						using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
						{
							return streamReader.ReadToEnd();
						}
					}
				}
			}
		}
	}
}
