namespace BaristaLabs.BaristaCore
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Represents a file-like object of immutable, raw data.
    /// </summary>
    /// <see cref="https://developer.mozilla.org/en-US/docs/Web/API/Blob"/>
    public class Blob
    {
        //Replace this with Span<byte>
        private readonly byte[] m_blob;
        private string m_type = string.Empty;

        public Blob(object[] array, JsObject options = null)
        {
            //Replace this with Span<byte>
            var data = new List<byte>();

            foreach (var arr in array)
            {
                switch (arr)
                {
                    case byte[] bytes:
                        data.AddRange(bytes);
                        break;
                    case Blob blob:
                        data.AddRange(blob.m_blob);
                        break;
                    default:
                        data.AddRange(Encoding.UTF8.GetBytes(arr.ToString()));
                        break;
                }
            }

            m_blob = data.ToArray();

            if (options != null)
            {
                if (options.HasProperty("type"))
                    m_type = options["type"].ToString();
            }
        }

        [BaristaIgnore]
        public Blob(byte[] data, string contentType = null)
        {
            m_blob = data ?? throw new ArgumentNullException(nameof(data));
            m_type = contentType;
        }

        /// <summary>
        /// Gets size in bytes of the blob.
        /// </summary>
        public double Size
        {
            get { return m_blob.LongLength; }
        }

        /// <summary>
        /// Gets the the MIME type of the blob. It returns an empty string if the type couldn't determined.
        /// </summary>
        public string Type
        {
            get { return m_type; }
        }

        /// <summary>
        /// Gets the underlying blob data.
        /// </summary>
        [BaristaIgnore]
        public byte[] Data
        {
            get { return m_blob; }
        }

        /// <summary>
        /// Returns a new Blob object containing the data in the specified range of bytes of the source Blob.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public Blob Slice(long? start = null, long? end = null, string contentType = "")
        {
            if (start.HasValue == false)
                start = 0;

            var indexFrom = start.Value;
            if (indexFrom < 0)
                indexFrom = m_blob.LongLength + start.Value;

            if (end.HasValue == false)
                end = m_blob.LongLength;

            if (indexFrom > end.Value)
                throw new ArgumentException($"Start index ({indexFrom}) cannot be after end ({end.Value}).");

            long length = end.Value - indexFrom;

            byte[] data = new byte[length];
            Array.Copy(m_blob, indexFrom, data, 0, length);

            var targetContentType = m_type;
            if (!String.IsNullOrWhiteSpace(contentType))
                targetContentType = contentType;

            return new Blob(data, targetContentType);
        }

        /// <summary>
        /// Decodes the blob into a UTF-8 string.
        /// </summary>
        /// <returns></returns>
        public string ToUtf8String()
        {
            return Encoding.UTF8.GetString(m_blob);
        }

        /// <summary>
        /// Converts the blob into a Base64 string encoding.
        /// </summary>
        /// <returns></returns>
        public string ToBase64EncodedByteArray()
        {
            return Convert.ToBase64String(m_blob);
        }

        public static Blob FromUtf8String(string data, string contentType = null)
        {
            return new Blob(Encoding.UTF8.GetBytes(data), contentType);
        }

        public static Blob FromBase64EncodedByteArray(string data, string contentType = null)
        {
            return new Blob(Convert.FromBase64String(data), contentType);
        }
    }
}
