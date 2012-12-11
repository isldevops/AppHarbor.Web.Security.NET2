using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace AppHarbor.Web.Security
{
	internal static class StringExtensions
	{
		public static byte[] GetByteArrayFromHexString(string s)
		{
			return SoapHexBinary.Parse(s).Value;
		}

		public static string GetHexString(byte[] b)
		{
			return new SoapHexBinary(b).ToString();
		}

        /// <summary>
        /// Indicates whether a specified string is null, empty, or consists only of white-space characters.
        /// </summary>
        /// 
        /// <returns>
        /// true if the <paramref name="value"/> parameter is null or <see cref="F:System.String.Empty"/>, or if <paramref name="value"/> consists exclusively of white-space characters.
        /// </returns>
        /// <param name="value">The string to test.</param>
        public static bool IsNullOrWhiteSpace(string value)
        {
            if (value == null)
                return true;
            for (int index = 0; index < value.Length; ++index)
            {
                if (!char.IsWhiteSpace(value[index]))
                    return false;
            }
            return true;
        }
	}
}
