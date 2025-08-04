using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecomeceAPI.Models.Serasa
{
  public interface ISearasaLayout
  {
    List<LayoutRule> Fields();
  }
}
