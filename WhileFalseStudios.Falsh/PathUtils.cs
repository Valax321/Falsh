namespace WhileFalseStudios.Falsh
{
    public class PathUtils
    {
        public static string MakeRelativePath(string workingDirectory, string fullPath)
        {
            string result = string.Empty;
            int offset;

            // this is the easy case.  The file is inside of the working directory.
            if (fullPath.StartsWith(workingDirectory))
            {
                // if you use workingDirectory.Length + 1, you don't see the leading backslash in non-root directories, but you will lose a leading char on
                // some directories/file names. This isn't, probably, an ideal solution, but it works.
                // hey-mikey, 10/16/2018
                return fullPath.Substring(workingDirectory.Length).Replace("\\", "").Trim();
            }

            // the hard case has to back out of the working directory
            string[] baseDirs = workingDirectory.Split(new char[] { ':', '\\', '/' });
            string[] fileDirs = fullPath.Split(new char[] { ':', '\\', '/' });

            // if we failed to split (empty strings?) or the drive letter does not match
            if (baseDirs.Length <= 0 || fileDirs.Length <= 0 || baseDirs[0] != fileDirs[0])
            {
                // can't create a relative path between separate harddrives/partitions.
                return fullPath;
            }

            // skip all leading directories that match
            for (offset = 1; offset < baseDirs.Length; offset++)
            {
                if (baseDirs[offset] != fileDirs[offset])
                    break;
            }

            // back out of the working directory
            for (int i = 0; i < (baseDirs.Length - offset); i++)
            {
                result += "..\\";
            }

            // step into the file path
            for (int i = offset; i < fileDirs.Length - 1; i++)
            {
                result += fileDirs[i] + "\\";
            }

            // append the file
            result += fileDirs[fileDirs.Length - 1];

            return result;
        }
    }
}
