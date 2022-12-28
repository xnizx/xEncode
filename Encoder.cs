using System;
using TextConverter;
using System.IO;

namespace xEncode
{
	/// <summary>
	/// Encoder에 대한 요약 설명입니다.
	/// </summary>
	public class Encoder
	{
		public static System.Text.Encoding[] encodings
			= {System.Text.Encoding.ASCII, 
				  System.Text.Encoding.BigEndianUnicode,
				  System.Text.Encoding.Default,
				  System.Text.Encoding.Unicode,
				  System.Text.Encoding.UTF7,
				  System.Text.Encoding.UTF8,
				  System.Text.Encoding.GetEncoding(949),
				  System.Text.Encoding.GetEncoding(932),
				  System.Text.Encoding.GetEncoding(936),
				  System.Text.Encoding.GetEncoding(950),
				  System.Text.Encoding.GetEncoding(1250),
				  System.Text.Encoding.GetEncoding(1251),
				  System.Text.Encoding.GetEncoding(1252),
				  System.Text.Encoding.GetEncoding(1253),
				  System.Text.Encoding.GetEncoding(1254),
				  System.Text.Encoding.GetEncoding(1255),
				  System.Text.Encoding.GetEncoding(1256),
				  System.Text.Encoding.GetEncoding(1257),
				  System.Text.Encoding.GetEncoding(1258),
				  System.Text.Encoding.GetEncoding(852),
				  System.Text.Encoding.GetEncoding(866),
				  System.Text.Encoding.GetEncoding(850),
				  System.Text.Encoding.GetEncoding(737),
				  System.Text.Encoding.GetEncoding(857),
				  System.Text.Encoding.GetEncoding(862),
				  System.Text.Encoding.GetEncoding(863),
				  System.Text.Encoding.GetEncoding(720),
				  System.Text.Encoding.GetEncoding(775),
				  System.Text.Encoding.GetEncoding(860)
			  };
		public static string[] encodingNames
			= {"ASCII","BigEndianUnicode","Default","Unicode (UTF-16)","UTF-7","UTF-8",
				"한국어[949]","일본어[932]","중국어 간체[936]","중국어 번체(Big5)[950]",
				"중앙 유럽어[1250]","키릴 자모[1251]","서유럽어[1252]","그리스어[1253]","터키어[1254]","히브리어[1255]","아랍어[1256]","발트어[1257]","베트남어[1258]",
				"중앙 유럽어(DOS)[852]","키릴 자모(DOS)[866]","서유럽어(DOS)[850]","그리스어(DOS)[737]","터키어(DOS)[857]","히브리어(DOS)[862]","프랑스어(캐나다)(DOS)[863]","아랍어(DOS)[720]","발트어(DOS)[775]","포르투갈어(DOS)[860]"
				};

		public static int[] encodingCodePages;

		static Encoder()
		{
				encodingCodePages = new Int32[encodings.Length];
			for(int i=0;i<encodings.Length;i++)
				encodingCodePages[i] = encodings[i].CodePage;
		}
		public static string EncodingToString(int codePage)
		{
			for(int i=0;i<encodingCodePages.Length;i++)
			{
				if(codePage == encodingCodePages[i])
					return encodingNames[i];
			}
			return "";
		}
		public static string EncodingToString(System.Text.Encoding encoding)
		{
			for(int i=0;i<encodings.Length;i++)
			{
				if(encoding == encodings[i])
					return encodingNames[i];
			}
			return "";
		}

		public static string Convert(string path, System.Text.Encoding encoding, bool preserveDate, bool preserveAtri, bool simulaton)
		{
			string content;
			FileAttributes fa;
			DateTime ctu;
			DateTime lwtu;

			//파일열기
			try
			{
				content = TextEncoding.ReadTextFile(path);
				fa = File.GetAttributes(path);
				ctu= File.GetCreationTimeUtc(path);
				lwtu= File.GetLastWriteTimeUtc(path);
			}
			catch(Exception)
			{
				return String.Format("파일을 열수 없습니다.");
			}

			//파일저장
			try
			{
				
				if(!simulaton)
				{
					System.Text.Encoding encoding2 = encoding;//2022.12.28
					if (encoding == System.Text.Encoding.UTF8)//2022.12.28 UTF-8 일경우
						encoding2 = new System.Text.UTF8Encoding(false);//2022.12.28 - BOM 제거
					StreamWriter sw = new StreamWriter(path,false, encoding2);
					sw.Write(content);
					sw.Flush();
					sw.Close();
				}
			}
			catch(Exception)
			{
				return String.Format("파일을 저장할수 없습니다.");
			}
			
			//속성 유지
			if(preserveDate)
			{
				File.SetCreationTimeUtc(path,ctu);
				File.SetLastWriteTimeUtc(path,lwtu);
			}
			if(preserveAtri)
				File.SetAttributes(path,fa);

			return "";
		}




	}
}
