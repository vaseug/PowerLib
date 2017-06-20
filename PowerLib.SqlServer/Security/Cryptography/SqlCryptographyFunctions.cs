using System;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

namespace PowerLib.SqlServer.Security.Cryptography
{
  public static class SqlCryptographyFunctions
  {
    #region Random generation functions

    [SqlFunction(Name = "cryptRandom", IsDeterministic = true)]
    public static SqlBytes Random(SqlInt32 count)
    {
      if (count.IsNull)
        return SqlBytes.Null;

      RNGCryptoServiceProvider random = new RNGCryptoServiceProvider();
      Byte[] data = new Byte[count.Value];
      random.GetBytes(data);
      return new SqlBytes(data);
    }

    [SqlFunction(Name = "cryptNonZeroRandom", IsDeterministic = true)]
    public static SqlBytes NonZeroRandom(SqlInt32 count)
    {
      if (count.IsNull)
        return SqlBytes.Null;

      RNGCryptoServiceProvider random = new RNGCryptoServiceProvider();
      Byte[] data = new Byte[count.Value];
      random.GetNonZeroBytes(data);
      return new SqlBytes(data);
    }

    #endregion
    #region Hash calculation functions

    [SqlFunction(Name = "cryptComputeHash", IsDeterministic = true)]
    public static SqlBytes ComputeHash(SqlBytes input, [SqlFacet(MaxSize = 128)] SqlString algorithmName)
    {
      if (input.IsNull || algorithmName.IsNull)
        return SqlBytes.Null;

      HashAlgorithm hashAlgorithm = HashAlgorithm.Create(algorithmName.Value);
      if (hashAlgorithm == null)
        return SqlBytes.Null;
      return new SqlBytes(hashAlgorithm.ComputeHash(input.Stream));
    }

    [SqlFunction(Name = "cryptVerifyHash", IsDeterministic = true)]
    public static SqlBoolean VerifyHash(SqlBytes input, SqlBytes hash, [SqlFacet(MaxSize = 128)] SqlString algorithmName)
    {
      if (input.IsNull || hash.IsNull || algorithmName.IsNull)
        return SqlBoolean.Null;

      HashAlgorithm hashAlgorithm = HashAlgorithm.Create(algorithmName.Value);
      if (hashAlgorithm == null)
        return SqlBoolean.Null;
      return new SqlBoolean(hashAlgorithm.ComputeHash(input.Stream).SequenceEqual(hash.Value));
    }

    [SqlFunction(Name = "cryptComputeKeyedHash", IsDeterministic = true)]
    public static SqlBytes ComputeKeyedHash(SqlBytes input, SqlBytes key, [SqlFacet(MaxSize = 128)] SqlString algorithmName)
    {
      if (input.IsNull || key.IsNull || algorithmName.IsNull)
        return SqlBytes.Null;

      KeyedHashAlgorithm hashAlgorithm = KeyedHashAlgorithm.Create(algorithmName.Value);
      if (hashAlgorithm == null)
        return SqlBytes.Null;
      hashAlgorithm.Key = key.Value;
      return new SqlBytes(hashAlgorithm.ComputeHash(input.Stream));
    }

    [SqlFunction(Name = "cryptVerifyKeyedHash", IsDeterministic = true)]
    public static SqlBoolean VerifyKeyedHash(SqlBytes input, SqlBytes key, SqlBytes hash, [SqlFacet(MaxSize = 128)] SqlString algorithmName)
    {
      if (input.IsNull || key.IsNull || hash.IsNull || algorithmName.IsNull)
        return SqlBoolean.Null;

      KeyedHashAlgorithm hashAlgorithm = KeyedHashAlgorithm.Create(algorithmName.Value);
      if (hashAlgorithm == null)
        return SqlBoolean.Null;
      hashAlgorithm.Key = key.Value;
      return new SqlBoolean(hashAlgorithm.ComputeHash(input.Stream).SequenceEqual(hash.Value));
    }

    #endregion
    #region Symmetric cryptography functions

    [SqlFunction(Name = "cryptEncryptSymmetric", IsDeterministic = true)]
    public static SqlBytes EncryptSymmetric(SqlBytes input, SqlBytes key, SqlBytes iv, [SqlFacet(MaxSize = 128)] SqlString algorithmName, SqlInt32 mode, SqlInt32 padding)
    {
      if (input.IsNull || key.IsNull || iv.IsNull || algorithmName.IsNull)
        return SqlBytes.Null;

      SymmetricAlgorithm symmAlgorithm = SymmetricAlgorithm.Create(algorithmName.Value);
      if (symmAlgorithm == null)
        return SqlBytes.Null;
      if (!mode.IsNull)
        symmAlgorithm.Mode = (CipherMode)mode.Value;
      if (!padding.IsNull)
        symmAlgorithm.Padding = (PaddingMode)padding.Value;
      using (ICryptoTransform encryptor = symmAlgorithm.CreateEncryptor(key.Value, iv.Value))
      using (MemoryStream ms = new MemoryStream())
      using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
      {
        input.Stream.CopyTo(cs);
        return new SqlBytes(ms);
      }
    }

    [SqlFunction(Name = "cryptDecryptSymmetric", IsDeterministic = true)]
    public static SqlBytes DecryptSymmetric(SqlBytes input, SqlBytes key, SqlBytes iv, [SqlFacet(MaxSize = 128)] SqlString algorithmName, SqlInt32 mode, SqlInt32 padding)
    {
      if (input.IsNull || key.IsNull || iv.IsNull || algorithmName.IsNull)
        return SqlBytes.Null;

      SymmetricAlgorithm symmAlgorithm = SymmetricAlgorithm.Create(algorithmName.Value);
      if (symmAlgorithm == null)
        return SqlBytes.Null;
      if (!mode.IsNull)
        symmAlgorithm.Mode = (CipherMode)mode.Value;
      if (!padding.IsNull)
        symmAlgorithm.Padding = (PaddingMode)padding.Value;
      using (ICryptoTransform decryptor = symmAlgorithm.CreateDecryptor(key.Value, iv.Value))
      using (MemoryStream ms = new MemoryStream())
      using (CryptoStream cs = new CryptoStream(input.Stream, decryptor, CryptoStreamMode.Read))
      {
        cs.CopyTo(ms);
        return new SqlBytes(ms);
      }
    }

    #endregion
  }
}
