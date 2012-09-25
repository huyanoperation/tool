#include <iostream>
#include "windows.h"
#include "CBase64.h"
#include "json.h"
using namespace std;



void writeTestFile( const char *path, string content )
{
   FILE *file = fopen( path, "wb" );
   if ( !file )
      return;
   int size = content.size();
   fwrite( content.c_str() , 1, size, file );
   fclose( file );
}


void findfile() 
{ 
	WIN32_FIND_DATA fd; 
	HANDLE hFind = FindFirstFile("*.json",&fd); 
	if(hFind!=INVALID_HANDLE_VALUE) 
	{ 
		do 
		{ 
			
			string file = readInputTestFile( fd.cFileName );

			Json::Value root;
			if( parseAndSaveValueTree( file, "", root ) )
			{ 
				char AssBuffer[64];
				sprintf( AssBuffer, "文件%s已经加过密或json格式错误！", fd.cFileName );
				MessageBox(NULL,AssBuffer,"",MB_OK);
			
			}
			else
			{
				string outStr;
				unsigned char buffer[100000];
				memcpy( buffer, file.c_str(),file.size() );
				CBase64::Encode( buffer, file.size(),  outStr );
				writeTestFile( fd.cFileName, outStr );
			}
			//CBase64::Encode( "dgasdg", 3,  outStr );

		}
		while(FindNextFile(hFind,&fd)); 
		FindClose(hFind); 
	} 
} 


void main()
{
	//CFileFind fd;
	findfile();
}