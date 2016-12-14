﻿using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace BuildSecureSystems.Framework.Encryption {

	/// <summary>
	/// This is not a good example of how to do this. Just KISS for the demo...
	/// </summary>
	public static class SampleEncryptor {

		public static string ENCRYPTED_VALUE_PREFIX = "//Encrypted:";
		public static byte[] KEY = new byte[16];
		public static byte[] HMAC_KEY = new byte[16];

		static SampleEncryptor() {
			// be smarter than this
			for (int i=0; i < KEY.Length; i++) {
				KEY[i] = 0x0;
			}
			for (int i = 0; i < HMAC_KEY.Length; i++) {
				HMAC_KEY[i] = 0x0;
			}
		}

		public static bool IsEncrypted(string value) {
			return value != null && value.StartsWith(ENCRYPTED_VALUE_PREFIX);
		}

		public static string Encrypt(string value) {
			if (IsEncrypted(value))
				return value;

			var encryptor = new Encryptor<AesEngine, Sha256Digest>(Encoding.UTF8, KEY, HMAC_KEY);
			var encrypted = ENCRYPTED_VALUE_PREFIX + encryptor.Encrypt(value);
			return encrypted;
		}

		public static string Decrypt(string value) {
			if (!IsEncrypted(value))
				return value;

			var encrypted = value.Substring(ENCRYPTED_VALUE_PREFIX.Length);

			var encryptor = new Encryptor<AesEngine, Sha256Digest>(Encoding.UTF8, KEY, HMAC_KEY);
			return encryptor.Decrypt(encrypted);
		}
	}
}