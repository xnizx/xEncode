using System;
using System.Text;
using System.IO;
using System.Collections;

namespace TextConverter
{
	/// <summary>
	/// 파일을 검사함. (static 메서드)
	/// </summary>
	public class FileClass
	{
		
		private static ArrayList Folders = new ArrayList();//폴더 리스트
		//파일 존재 여부를 확인
		public static bool IsFileExist(string path)
		{
			return File.Exists(path);
		}
		public static bool IsFileExist(string[] paths)
		{
			foreach(string path in paths)
			{
				if(!(IsFileExist(path)))
					return false;
			}
			return true;
		}
		//파일 존재 여부를 확인, 오류난 파일명을 반환
		public static bool IsFileExist(string[] paths, ref string name)
		{
			foreach(string path in paths)
			{
				if(!(IsFileExist(path)))
				{
					name = path;
					return false;
				}
			}
			return true;	
		}
		//모든 서브 폴더명을 ArrayList로 얻습니다.
		public static ArrayList GetSubFolders(string path)
		{
			Folders.Clear();
			AddSubFolders(path);
			Folders.RemoveAt(0);
			return Folders;
		}
		private static void AddSubFolders(string path)
		{
			Folders.Add(path);
			DirectoryInfo di = new DirectoryInfo(path);
			foreach(DirectoryInfo di2 in di.GetDirectories("*.*"))
			{
				AddSubFolders(di2.FullName);
			}
		}
		//디렉토리명인지 여부를 검사합니다.
		public static bool IsDirName(string path)
		{
			if(Directory.Exists(path))
				return true;
			return false;
		}
	}


	/// <summary>
	/// 파일의 텍스트 인코딩을 변환, 출력함.
	/// </summary>
	public class Converter
	{
		private string[] filenames; //파일목록을 저장할 string배열
		private System.Text.Encoding encoTo = System.Text.Encoding.UTF8; //변환될 인코딩, 기본값 UTF8
		private System.Text.Encoding encoNow; //기존 파일의 인코딩
		private string fileContent; //현재 파일 내용

		//변환할 파일 리스트 설정
		public void SetFilename(string[] filenames)
		{
			this.filenames=filenames;
		}
		public void SetFilename(string filename)
		{
			this.filenames = new string[0];
			this.filenames[0]=filename;
		}

		//변환될 인코딩 설정
		public void SetEncoding(System.Text.Encoding enco)
		{
			this.encoTo=enco;
		}
		public void SetEncoding(int enco)
		{
			this.encoTo=System.Text.Encoding.GetEncoding(enco);
		}
		public void SetEncoding(string enco)
		{
			this.encoTo=System.Text.Encoding.GetEncoding(enco);
		}

		//filenames배열에 있는 파일을 변환시작함
		public void StartConvert()
		{
			Print("/'"+TextEncoding.EncodingToString(encoTo)+"/'인코딩으로 변환을 시작합니다.\n\n");
			for(int i=0;i<filenames.Length;i++)
			{
				Print("["+i.ToString()+"] "+filenames[i]+"\n");
				this.Convert(filenames[i]);
			}
		}

		//파일 변환
		public void Convert(string filename)
		{
			try
			{
				//인코딩 값 가져오기
				this.encoNow=TextEncoding.Get(filename);
				if(encoNow==encoTo)
				{
					Print(" 이미 /'"+TextEncoding.EncodingToString(encoNow)+"/'이므로 생략합니다.\n");
				}
				else
				{
					//불러오기
					LoadFile(filename);
					//저장하기
					SaveFile(filename);
					Print(" /'"+TextEncoding.EncodingToString(encoNow)+"/'에서 /'"+TextEncoding.EncodingToString(encoTo)+"/'으로 변환 되었습니다.\n");
				}
			}
			catch(Exception e)
			{
				Print(" 오류: "+e.Message);
			}
		}
		public void Convert(string filename, System.Text.Encoding enco)
		{
			this.encoTo = enco;
			this.Convert(filename);
		}

		//인코딩 체크 출력
		public void Check(string filename)
		{
			try
			{
				//인코딩 값 가져오기
				this.encoNow=TextEncoding.Get(filename);
				Print("/'"+TextEncoding.EncodingToString(encoNow)+"/' 인코딩 입니다.\n");
			}
			catch(Exception e)
			{
				Print(" 오류: "+e.Message);
			}
		}

		//진행 상황 출력
		protected virtual void Print(string message)
		{
			Console.Write(message);
		}

		//파일 열기
		protected virtual void LoadFile(string filename)
		{
			StreamReader sr = new StreamReader(filename,encoNow);
			this.fileContent=sr.ReadToEnd();
			sr.Close();
		}

		//파일 저장하기
		protected virtual void SaveFile(string filename)
		{
			System.Text.Encoding encoTo2 = encoTo;//2022.12.28
			if (encoTo == System.Text.Encoding.UTF8)//2022.12.28 UTF-8 일경우
				encoTo2 = new UTF8Encoding(false);//2022.12.28 - BOM 제거
			StreamWriter sw = new StreamWriter(filename,false,encoTo2);
			sw.Write(fileContent);
			sw.Flush();
			sw.Close();
		}


	}


	/// <summary>
	/// TextConverter.TextEnoding 클래스
	/// 특정 텍스트 파일의 인코딩을 알아 냅니다.
	/// </summary>
	public class TextEncoding
	{
		private const int readLength = 100; //읽어들일 바이트 길이
		private static byte[] fileContent = new byte[readLength]; //읽어들인 내용
		private static System.Text.Encoding defaultEncoding = System.Text.Encoding.Default; //인코딩 기본값.

		/// <summary>
		/// 텍스트파일 내의 인코딩을 검사합니다.
		/// </summary>
		/// <param name="filename">검사할 텍스트 파일명</param>
		/// <returns>텍스트 인코딩</returns>
		private static System.Text.Encoding CheckUtf8(byte[] buffer, int size)
		{
			bool _nullSuggestsBinary = true;
			// UTF8 Valid sequences
			// 0xxxxxxx  ASCII
			// 110xxxxx 10xxxxxx  2-byte
			// 1110xxxx 10xxxxxx 10xxxxxx  3-byte
			// 11110xxx 10xxxxxx 10xxxxxx 10xxxxxx  4-byte
			//
			// Width in UTF8
			// Decimal      Width
			// 0-127        1 byte
			// 194-223      2 bytes
			// 224-239      3 bytes
			// 240-244      4 bytes
			//
			// Subsequent chars are in the range 128-191
			var onlySawAsciiRange = true;
			uint pos = 0;

			while (pos < size)
			{
				byte ch = buffer[pos++];

				if (ch == 0 && _nullSuggestsBinary)
				{
					return defaultEncoding;
				}

				int moreChars;
				if (ch <= 127)
				{
					// 1 byte
					moreChars = 0;
				}
				else if (ch >= 194 && ch <= 223)
				{
					// 2 Byte
					moreChars = 1;
				}
				else if (ch >= 224 && ch <= 239)
				{
					// 3 Byte
					moreChars = 2;
				}
				else if (ch >= 240 && ch <= 244)
				{
					// 4 Byte
					moreChars = 3;
				}
				else
				{
					return defaultEncoding; // Not utf8
				}

				// Check secondary chars are in range if we are expecting any
				while (moreChars > 0 && pos < size)
				{
					onlySawAsciiRange = false; // Seen non-ascii chars now

					ch = buffer[pos++];
					if (ch < 128 || ch > 191)
					{
						return defaultEncoding; // Not utf8
					}

					--moreChars;
				}
			}

			// If we get to here then only valid UTF-8 sequences have been processed

			// If we only saw chars in the range 0-127 then we can't assume UTF8 (the caller will need to decide)
			return onlySawAsciiRange ? defaultEncoding : System.Text.Encoding.UTF8;
		}
	
		public static System.Text.Encoding Get(string filename)
		{
			try
			{
				//파일 읽기
				FileStream fs = new FileStream(filename,FileMode.Open,FileAccess.Read);
				fs.Seek(0,SeekOrigin.Begin);
				for(int i=0;i<readLength;i++)
				{
					int temp;
					temp=fs.ReadByte();
					if (temp==-1)
						temp=32;
					fileContent[i]=(byte)temp;
				}
				fs.Close();

				//BigEndianUnicode
				if(fileContent[0]==0xFE && fileContent[1]==0xFF)
					return System.Text.Encoding.BigEndianUnicode;

				//Unicode
				if(fileContent[0]==0xFF && fileContent[1]==0xFE)
					return System.Text.Encoding.Unicode;

				//UTF8
				if(fileContent[0]==0xEF && fileContent[1]==0xBB && fileContent[2]==0xBF)
					return System.Text.Encoding.UTF8;

				return CheckUtf8(fileContent, readLength);
				//ASCII
				//UTF7
				//Default
				return defaultEncoding;
				
			}
			catch(FileNotFoundException e)
			{
				throw new Exception("파일을 찾을수 없습니다.",e);
			}
			catch(Exception e)
			{
				throw new Exception("파일을 읽을수 없습니다.",e);
			}
		}
		
		//배열도 사용가능 : 오류 반환 불가
		public static System.Text.Encoding[] Get(string[] filename)
		{
			System.Text.Encoding[] enco = new System.Text.Encoding[filename.Length];
			for(int count=0;count<filename.Length;count++)
				enco[count] = Get(filename[count]);	
			return enco;
		}

		/// <summary>
		/// 인코딩 명칭을 출력합니다.
		/// </summary>
		/// <param name="enco">출력할 인코딩</param></param>
		/// <returns>인코딩 명칭</returns>
		public static string EncodingToString(System.Text.Encoding enco)
		{
			if (enco==System.Text.Encoding.ASCII)
				return "ASCII";
			if (enco==System.Text.Encoding.BigEndianUnicode )
				return "BigEndianUnicode";
			if (enco==System.Text.Encoding.Default )
				return "Default";
			if (enco==System.Text.Encoding.Unicode )
				return "Unicode";
			if (enco==System.Text.Encoding.UTF7 )
				return "UTF-7";
			if (enco==System.Text.Encoding.UTF8 )
				return "UTF-8";
			return enco.CodePage.ToString();
		}
		/// <summary>
		/// 해당파일의 인코딩을 찾아내어 읽습니다.
		/// </summary>
		/// <param name="filename">파일명</param>
		/// <returns>텍스트 내용</returns>
		public static string ReadTextFile(string filename)
		{
			StreamReader sr = new StreamReader(filename,Get(filename));
			string temp = sr.ReadToEnd();
			sr.Close();
			return temp;
		}
		/// <summary>
		/// 인코딩 기본값을 설정합니다.
		/// </summary>
		/// <param name="enco">인코딩값</param>
		public static void SetDefEncoding(System.Text.Encoding enco)
		{
			try
			{
				defaultEncoding = enco;
			}
			catch
			{
			}
		}
	}

}
