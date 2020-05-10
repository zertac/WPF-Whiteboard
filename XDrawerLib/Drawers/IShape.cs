using System.Windows;

namespace XDrawerLib.Drawers
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