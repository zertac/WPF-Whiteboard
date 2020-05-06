using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPDFDoc.Helpers
{
  public static class Extensions
  {
    public static T ToType<T>(this object o)
    {
      return (T) o;
    }
  }
}
