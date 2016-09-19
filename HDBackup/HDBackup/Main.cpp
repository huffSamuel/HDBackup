#define VERSION "1.0"
#include <iostream>
using std::cout;
using std::cin;
using std::endl;

#include <string>
#include <ctime>
using std::time;

#include <Windows.h>

int main(int argc, char *argv[])
{
	int out = 0;
	char _source[64] = { 0 };
	char _dest[64] = { 0 };
	char _id[64] = { 0 };

	time_t t = time(0);
	struct tm now;
	localtime_s(&now, &t);

	if (argc == 1)
	{
		// User input
		cout << "User name and PC number: ";
		cin.getline(_id, 64);
		cout << "Source drive letter: ";
		cin.getline(_source, 64);
		// End user input
		
	}
	else if (argc == 2)
	{
		// Display help
		if (!strcmp(argv[1], "/h") || !strcmp(argv[1], "-h"))
		{
			cout << "\n------------------------------ "
				<< "\nService Desk Backup version " << VERSION << endl;
			cout << "HDBackup.exe " << "<user & PC#> " << "<source drive> " << endl;
			cout << "... or HDBackup.exe to be prompted for input " << endl;
			cout << "\nTo modify destination drive change config.ini in the source directory." << endl;
		}
		else if (!strcmp(argv[1], "/s") || !strcmp(argv[1], "-s"))
		{
			cout << endl;
		}
		else if (!strcmp(argv[1], "/v") || !strcmp(argv[1], "-v"))
		{
			cout << "Version " << VERSION << endl;
		}
	}
	else if (argc == 3)
	{
		cout << "User, src input" << endl;
	}
	else
	{
		cout << "ERROR: Incorrect parameters. Try HDBackup.exe /h for help." << endl;
		out = 1;
	}
	return out;
}

int runBackup(char * destString, char * source)
{
	int out = 0;

	return out;
}
