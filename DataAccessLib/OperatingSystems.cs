using System;

namespace VeraWAF.WebPages.Dal
{
    public class OperatingSystems
    {
        private readonly String[] _operatingSystemsCategories =
            {
                "Unix -> IBM AIX",
                "Linux -> DSPnano RTOS",
                "Unix -> Hewlett-Packard UniX (HP-UX)",
                "Unix -> Mac OS X",
                "Unix -> MINIX",
                "Unix -> QNX",
                "Other -> RTEMS",
                "Other -> Integrity",
                "Unix -> Solaris",
                "Unix -> Tru64 UNIX",
                "Unix -> UnixWare",
                "Other -> velOSity",
                "Other -> VxWorks",
                "Other -> Haiku",
                "Unix -> FreeBSD",
                "Linux",
                "Unix -> NetBSD",
                "Other -> Nucleus RTOS",
                "Unix -> OpenBSD",
                "Unix -> OpenSolaris",
                "Other -> PikeOS",
                "Other -> RTEMS",
                "Other -> Cygwin",
                "Unix",
                //"Windows",
                "Other -> Symbian",
                "Other -> z/OS",
                "Linux -> Ubuntu",
                "Linux -> Mint",
                "Linux -> Fedora",
                "Linux -> Debian",
                "Linux -> OpenSUSE",
                "Linux -> Arch",
                "Linux -> PCLinuxOS",
                "Linux -> Puppy",
                "Linux -> CentOS",
                "Linux -> Sabayon",
                "Linux -> Mandriva",
                "Linux -> Slackware",
                "Linux -> Chakra",
                "Linux -> Scientific",
                "Linux -> MEPIS"
            };

        public OperatingSystems()
        {
            Array.Sort(_operatingSystemsCategories);
        }

        public String[] GetCategories
        {
            get { return _operatingSystemsCategories; }
        }
    }
}