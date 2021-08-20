using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;

namespace SamsungFumoClient.Utils
{
    public static class AssertUtils
    {
        public static void AreRequiredPropsNonNull(object cls)
        {
            foreach (PropertyInfo prop in cls.GetType().GetProperties())
            {
                object[] attribute = prop.GetCustomAttributes(typeof(RequiredAttribute), true);

                if (attribute.Length > 0)
                {
                    Debug.Assert(prop.GetValue(cls, null) != null,
                        $"{cls.GetType().FullName}: {prop.Name} was unexpectedly set to null");
                }
            }
        }
    }
}