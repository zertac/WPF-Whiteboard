using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace XPDFDoc
{
  public interface IShape
  {
    void Create(Point e);
    void Update(Point e);
    void Finish();
    void Cancel();
    bool IsSelected { get; set; }
  }
}