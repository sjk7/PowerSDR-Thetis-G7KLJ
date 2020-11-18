// This is an independent project of an individual developer. Dear PVS-Studio, please check it.

// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
/*  version.c

MW0LGE

*/

#include "cmcomm.h"
#include "version.h"

PORT
int GetCMVersion()
{
	// MW0LGE version number now stored in Thetis->Versions.cs file, to keep shared
	// version numbers between c/c#

	return _CMASTER_VERSION;
}

