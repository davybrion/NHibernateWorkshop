using System;
using Northwind.Enums;

namespace Northwind.Components
{
    public class ImageInfo : IEquatable<ImageInfo>
    {
        public string Path { get; private set; }
        public ImageType ImageType { get; private set; }

        private ImageInfo() {}

        public ImageInfo(string path, ImageType imageType)
        {
            Path = path;
            ImageType = imageType;
        }

        public bool Equals(ImageInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Path, Path) && Equals(other.ImageType, ImageType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(ImageInfo)) return false;
            return Equals((ImageInfo)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Path.GetHashCode() * 397) ^ ImageType.GetHashCode();
            }
        }

        public static bool operator ==(ImageInfo left, ImageInfo right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ImageInfo left, ImageInfo right)
        {
            return !Equals(left, right);
        }
    }
}