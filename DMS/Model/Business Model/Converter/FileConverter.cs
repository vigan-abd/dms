using Model.Business.ViewModel;
using Model.Domain_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Business.Converter
{
    public static class FileConverter
    {
        public static File ViewModelToFile(FileViewModel vm)
        {
            return new File()
            {
                AccessLevel = vm.AccessLevel,
                Keywords = vm.Keywords,
                LastModified = vm.LastModified,
                LastVersion = vm.LastVersion,
                RelativeDirectory = vm.RelativeDirectory,
                ShortDesc = vm.ShortDesc,
                Title = vm.Title,
                FileID = vm.FileID,
                ExternalLink = vm.ExternalLink
            };
        }

        public static FileViewModel FileToViewModel(File m)
        {
            return new FileViewModel()
            {
                AccessLevel = m.AccessLevel,
                Keywords = m.Keywords,
                LastModified = m.LastModified,
                LastVersion = m.LastVersion,
                RelativeDirectory = m.RelativeDirectory,
                ShortDesc = m.ShortDesc,
                Title = m.Title,
                FileID = m.FileID,
                ExternalLink = m.ExternalLink
            };
        }

        public static FileVersion ViewModelToFileVer(FileVersionViewModel vm)
        {
            return new FileVersion()
            {
                FileID = vm.FileID,
                Name = vm.Name,
                VerNo = vm.VerNo
            };
        }

        public static FileVersionViewModel FileVerToViewModel(FileVersion model)
        {
            return new FileVersionViewModel()
            {
                FileID = model.FileID,
                VerNo = model.VerNo,
                Name = model.Name,
                RelativeDirectory = model.File.RelativeDirectory
            };
        }

        public static SimpleFileViewModel FileToSimpleViewModel(File model)
        {
            SimpleFileViewModel vm = new SimpleFileViewModel()
            {
                FileID = model.FileID,
                LastModified = model.LastModified,
                LastVersion = model.LastVersion,
                OwnerID = model.Owner.UserID,
                ShortDesc = model.ShortDesc,
                OwnerName = model.Owner.Username,
                Title = model.Title
            };
            return vm;
        }
    }
}
