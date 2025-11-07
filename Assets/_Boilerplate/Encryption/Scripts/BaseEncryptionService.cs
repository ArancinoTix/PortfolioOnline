using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace U9.Encryption
{
    public abstract class BaseEncryptionService : MonoBehaviour
    {
        public abstract string Encrypt(string value);
        public abstract string Decrypt(string value);
    }
}
