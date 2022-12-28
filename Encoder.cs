using System;
using TextConverter;
using System.IO;

namespace xEncode
{
	/// <summary>
	/// Encoder�� ���� ��� �����Դϴ�.
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
				"�ѱ���[949]","�Ϻ���[932]","�߱��� ��ü[936]","�߱��� ��ü(Big5)[950]",
				"�߾� ������[1250]","Ű�� �ڸ�[1251]","��������[1252]","�׸�����[1253]","��Ű��[1254]","���긮��[1255]","�ƶ���[1256]","��Ʈ��[1257]","��Ʈ����[1258]",
				"�߾� ������(DOS)[852]","Ű�� �ڸ�(DOS)[866]","��������(DOS)[850]","�׸�����(DOS)[737]","��Ű��(DOS)[857]","���긮��(DOS)[862]","��������(ĳ����)(DOS)[863]","�ƶ���(DOS)[720]","��Ʈ��(DOS)[775]","����������(DOS)[860]"
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

			//���Ͽ���
			try
			{
				content = TextEncoding.ReadTextFile(path);
				fa = File.GetAttributes(path);
				ctu= File.GetCreationTimeUtc(path);
				lwtu= File.GetLastWriteTimeUtc(path);
			}
			catch(Exception)
			{
				return String.Format("������ ���� �����ϴ�.");
			}

			//��������
			try
			{
				
				if(!simulaton)
				{
					System.Text.Encoding encoding2 = encoding;//2022.12.28
					if (encoding == System.Text.Encoding.UTF8)//2022.12.28 UTF-8 �ϰ��
						encoding2 = new System.Text.UTF8Encoding(false);//2022.12.28 - BOM ����
					StreamWriter sw = new StreamWriter(path,false, encoding2);
					sw.Write(content);
					sw.Flush();
					sw.Close();
				}
			}
			catch(Exception)
			{
				return String.Format("������ �����Ҽ� �����ϴ�.");
			}
			
			//�Ӽ� ����
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
