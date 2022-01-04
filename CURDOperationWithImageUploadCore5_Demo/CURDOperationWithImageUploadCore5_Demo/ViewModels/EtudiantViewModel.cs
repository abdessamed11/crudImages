using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CURDOperationWithImageUploadCore5_Demo.ViewModels
{
    public class EtudiantViewModel : EditImageViewModel
    {
        [Required]
        public string Name { get; set; }
    }
}
