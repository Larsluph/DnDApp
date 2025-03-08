using System;
using System.Collections.Generic;
using Microsoft.VisualBasic;
using System.Runtime.InteropServices;

namespace DnDApp
{
    internal class NativeFileIO
    {
        private enum FO_Func : uint
        {
            FO_NONE = 0,
            /// <summary>
            /// Move the files specified in pFrom to the location specified in pTo.
            /// </summary>
            FO_MOVE = 1,
            /// <summary>
            /// Copy the files specified in the pFrom member to the location specified in the pTo member.
            /// </summary>
            FO_COPY = 2,
            /// <summary>
            /// Delete the files specified in pFrom.
            /// </summary>
            FO_DELETE = 3,
            /// <summary>
            /// Rename the file specified in pFrom. You cannot use this flag to rename multiple files with a single function call. Use FO_MOVE instead.
            /// </summary>
            FO_RENAME = 4
        }

        [Flags] private enum FOF_Flags : ushort
        {
            FOF_NONE = 0,
            /// <summary>
            /// The pTo member specifies multiple destination files (one for each source file in pFrom) rather than one directory where all source files are to be deposited.
            /// </summary>
            FOF_MULTIDESTFILES = 1,
            /// <summary>
            /// Not used.
            /// </summary>
            FOF_CONFIRMMOUSE = 2,
            /// <summary>
            /// Do not display a progress dialog box.
            /// </summary>
            FOF_SILENT = 4,
            /// <summary>
            /// Give the file being operated on a new name in a move, copy, or rename operation if a file with the target name already exists at the destination.
            /// </summary>
            FOF_RENAMEONCOLLISION = 8,
            /// <summary>
            /// Respond with Yes to All for any dialog box that is displayed.
            /// </summary>
            FOF_NOCONFIRMATION = 16,
            /// <summary>
            /// If FOF_RENAMEONCOLLISION is specified and any files were renamed, assign a name mapping object that contains their old and new names to the hNameMappings member. This object must be freed using SHFreeNameMappings when it is no longer needed.
            /// </summary>
            FOF_WANTMAPPINGHANDLE = 32,
            /// <summary>
            /// Preserve undo information, if possible.
            /// </summary>
            FOF_ALLOWUNDO = 64,
            /// <summary>
            /// Perform the operation only on files (not on folders) if a wildcard file name (.) is specified.
            /// </summary>
            FOF_FILESONLY = 128,
            /// <summary>
            /// Display a progress dialog box but do not show individual file names as they are operated on.
            /// </summary>
            FOF_SIMPLEPROGRESS = 256,
            /// <summary>
            /// Do not ask the user to confirm the creation of a new directory if the operation requires one to be created.
            /// </summary>
            FOF_NOCONFIRMMKDIR = 512,
            /// <summary>
            /// Do not display a dialog to the user if an error occurs.
            /// </summary>
            FOF_NOERRORUI = 1024,
            /// <summary>
            /// Version 4.71. Do not copy the security attributes of the file. The destination file receives the security attributes of its new folder.
            /// </summary>
            FOF_NOCOPYSECURITYATTRIBS = 2048,
            /// <summary>
            /// Only perform the operation in the local directory. Do not operate recursively into subdirectories, which is the default behavior.
            /// </summary>
            FOF_NORECURSION = 4096,
            /// <summary>
            /// Version 5.0. Do not move connected files as a group. Only move the specified files.
            /// </summary>
            FOF_NO_CONNECTED_ELEMENTS = 8192,
            /// <summary>
            /// Version 5.0. Send a warning if a file is being permanently destroyed during a delete operation rather than recycled. This flag partially overrides FOF_NOCONFIRMATION.
            /// </summary>
            FOF_WANTNUKEWARNING = 16384,
            /// <summary>
            /// Not used.
            /// </summary>
            FOF_NORECURSEREPARSE = 32768,
            /// <summary>
            /// Windows Vista. Perform the operation silently, presenting no UI to the user. This is equivalent to FOF_SILENT | FOF_NOCONFIRMATION | FOF_NOERRORUI | FOF_NOCONFIRMMKDIR.
            /// </summary>
            FOF_NO_UI = FOF_SILENT | FOF_NOCONFIRMATION | FOF_NOERRORUI | FOF_NOCONFIRMMKDIR
        }

        private struct SHFILEOPSTRUCT
        {
            /// <summary>
            /// A window handle to the dialog box to display information about the status of the file operation.
            /// </summary>
            public IntPtr hwnd;
            /// <summary>
            /// A value that indicates which operation to perform.
            /// </summary>
            public FO_Func wFunc;
            /// <summary>
            /// <para>A pointer to one or more source file names. These names should be fully qualified paths to prevent unexpected results.</para>
            /// <para>Standard MS-DOS wildcard characters, such as "*", are permitted only in the file-name position.Using a wildcard character elsewhere in the string will lead to unpredictable results.</para>
            /// <para>Although this member is declared as a single null-terminated string, it is actually a buffer that can hold multiple null-delimited file names. Each file name is terminated by a single NULL character. The last file name is terminated with a double NULL character ("\0\0") to indicate the end of the buffer.</para>
            /// <para><b>Note:</b> This string must be double-null terminated.</para>
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pFrom;
            /// <summary>
            /// <para>A pointer to the destination file or directory name. This parameter must be set to NULL if it is not used. Wildcard characters are not allowed. Their use will lead to unpredictable results.</para>
            /// <para>Like pFrom, the pTo member is also a double-null terminated string and is handled in much the same way.</para>
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pTo;
            /// <summary>
            /// Flags that control the file operation.
            /// </summary>
            public FOF_Flags fFlags;
            /// <summary>
            /// When the function returns, this member contains TRUE if any file operations were aborted before they were completed; otherwise, FALSE. An operation can be manually aborted by the user through UI or it can be silently aborted by the system if the FOF_NOERRORUI or FOF_NOCONFIRMATION flags were set.
            /// </summary>
            public bool fAnyOperationsAborted;
            /// <summary>
            /// When the function returns, this member contains a handle to a name mapping object that contains the old and new names of the renamed files. This member is used only if the fFlags member includes the FOF_WANTMAPPINGHANDLE flag.
            /// </summary>
            public IntPtr hNameMappings;
            /// <summary>
            /// A pointer to the title of a progress dialog box. This is a null-terminated string. This member is used only if fFlags includes the FOF_SIMPLEPROGRESS flag.
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpszProgressTitle;
        }

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern int SHFileOperation([In, Out] ref SHFILEOPSTRUCT lpFileOp);

        public static int Move(List<string> sSource, string sTarget)
        {
            SHFILEOPSTRUCT _shFile = new()
            {
                wFunc = FO_Func.FO_MOVE,
                pFrom = string.Join(Constants.vbNullChar, sSource) + Constants.vbNullChar,
                pTo = sTarget,
                fFlags = FOF_Flags.FOF_ALLOWUNDO | FOF_Flags.FOF_NOCONFIRMMKDIR
            };
            return SHFileOperation(ref _shFile);
        }
        public static int Move(List<string> sSource, List<string> sTarget)
        {
            SHFILEOPSTRUCT _shFile = new()
            {
                wFunc = FO_Func.FO_MOVE,
                pFrom = string.Join(Constants.vbNullChar, sSource) + Constants.vbNullChar,
                pTo = string.Join(Constants.vbNullChar, sTarget) + Constants.vbNullChar,
                fFlags = FOF_Flags.FOF_ALLOWUNDO | FOF_Flags.FOF_NOCONFIRMMKDIR | FOF_Flags.FOF_MULTIDESTFILES
            };
            return SHFileOperation(ref _shFile);
        }

        public static int Copy(List<string> sSource, string sTarget)
        {
            SHFILEOPSTRUCT _shFile = new()
            {
                wFunc = FO_Func.FO_COPY,
                pFrom = string.Join(Constants.vbNullChar, sSource) + Constants.vbNullChar,
                pTo = sTarget,
                fFlags = FOF_Flags.FOF_ALLOWUNDO | FOF_Flags.FOF_NOCONFIRMMKDIR
            };
            return SHFileOperation(ref _shFile);
        }
        public static int Copy(List<string> sSource, List<string> sTarget)
        {
            SHFILEOPSTRUCT _shFile = new()
            {
                wFunc = FO_Func.FO_COPY,
                pFrom = string.Join(Constants.vbNullChar, sSource) + Constants.vbNullChar,
                pTo = string.Join(Constants.vbNullChar, sTarget) + Constants.vbNullChar,
                fFlags = FOF_Flags.FOF_ALLOWUNDO | FOF_Flags.FOF_NOCONFIRMMKDIR | FOF_Flags.FOF_MULTIDESTFILES
            };
            return SHFileOperation(ref _shFile);
        }

        public static int Delete(List<string> sSource)
        {
            SHFILEOPSTRUCT _shFile = new()
            {
                wFunc = FO_Func.FO_DELETE,
                pFrom = string.Join(Constants.vbNullChar, sSource) + Constants.vbNullChar,
                fFlags = FOF_Flags.FOF_ALLOWUNDO
            };
            return SHFileOperation(ref _shFile);
        }

        public static int Rename(string sSource, string sTarget)
        {
            SHFILEOPSTRUCT _shFile = new()
            {
                wFunc = FO_Func.FO_RENAME,
                pFrom = sSource + Constants.vbNullChar,
                pTo = sTarget,
                fFlags = FOF_Flags.FOF_ALLOWUNDO | FOF_Flags.FOF_NOCONFIRMMKDIR
            };
            return SHFileOperation(ref _shFile);
        }
    }
}
