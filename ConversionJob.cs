﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SinsDataConverter
{
	class ConversionJob
	{
		private bool _directionToTxt;
		private FileInfo _exe;
		private FileInfo _outputFile;
		private bool _overwrite;
		private FileInfo _sourceFile;

		public bool ConvertToBin
		{
			get
			{
				return !_directionToTxt;
			}
			set
			{
				if (value)
				{
					_directionToTxt = false;
				}
				else
				{
					_directionToTxt = true;
				}
			}
		}

		public bool ConvertToTxt
		{
			get
			{
				return _directionToTxt;
			}
			set
			{
				if (value)
				{
					_directionToTxt = true;
				}
				else
				{
					_directionToTxt = false;
				}
			}
		}

		public string ExePath
		{
			get
			{
				return _exe.FullName;
			}
			set
			{
				var file = new FileInfo(value);
				if (!file.Exists)
				{
					throw new FileNotFoundException(file.Name + " could not be found", file.FullName);
				}
				_exe = file;
			}
		}

		public string FileName
		{
			get
			{
				return _sourceFile.Name;
			}
		}

		public bool IsInPlace
		{
			get
			{
				return _sourceFile.FullName == _outputFile.FullName;
			}
			set
			{
				if (value)
				{
					_outputFile = new FileInfo(_sourceFile.FullName);
				}
			}
		}

		public string OutputPath
		{
			get
			{
				return _outputFile.FullName;
			}
			set
			{
				var file = new FileInfo(value);
				if (file.Exists)
				{
					_overwrite = true;
				}
			}
		}

		public string SourcePath
		{
			get
			{
				return _sourceFile.FullName;
			}
			set
			{
				var file = new FileInfo(value);
				if (!file.Exists)
				{
					throw new FileNotFoundException(file.Name + " could not be found", file.FullName);
				}
				_sourceFile = file;
			}
		}

		public FileType Type
		{
			get
			{
				return FileTypes.GetFromExtension(_sourceFile.Extension);
			}
		}

		public bool WillOverwrite
		{
			get
			{
				return _overwrite;
			}
		}

		public static ConversionJob Create(FileInfo file, DirectoryInfo output, FileInfo exe, bool convertToTxt = false)
		{
			return new ConversionJob()
			{
				ConvertToTxt = convertToTxt,
				ExePath = exe.FullName,
				OutputPath = file.FullName.Replace(file.DirectoryName, output.FullName),
				SourcePath = file.FullName
			};
		}

		public static List<ConversionJob> Create(DirectoryInfo folder, DirectoryInfo output, FileInfo exe, bool convertToTxt = false)
		{
			var files = new List<FileInfo>();
			foreach (var type in FileTypes.All)
			{
				files.AddRange(folder.EnumerateFiles("*" + type.Extension, SearchOption.AllDirectories));
			}
			var jobs = new List<ConversionJob>();
			foreach (var file in files)
			{
				jobs.Add(new ConversionJob()
				{
					ConvertToTxt = convertToTxt,
					ExePath = exe.FullName,
					OutputPath = file.FullName.Replace(folder.FullName, output.FullName),
					SourcePath = file.FullName
				});
			}
			return jobs;
		}

		public override string ToString()
		{
			return $"\"{ExePath}\" {Type.Name} \"{SourcePath}\" \"{OutputPath}\"{(ConvertToTxt ? " txt" : "")}";
		}
	}
}