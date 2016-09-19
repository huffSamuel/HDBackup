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
		cin.ignore(INT_MAX);
		cout << "Source drive letter: ";
		cin.getline(_source, 64);
		cin.ignore(INT_MAX);
		// End user input
		out = backup();

	}
	else if (argc == 2 && (!strcmp(argv[1], "/h") || !strcmp(argv[1], "-h")))
	{
		// Display help
		
		cout << "\n------------------------------ "
			 << "\nService Desk Backup version " << VERSION << endl;
		cout << "HDBackup.exe " << "<user & PC#> " << "<source drive> " << endl;
		cout << "... or HDBackup.exe to be prompted for input " << endl;
		cout << "\nTo modify destination drive change config.ini in the source directory." << endl;
	}
	else if (argc == 3)
	{
		cout << "User, src input" << endl;
		out = backup();
	}
	else
	{
		cout << "ERROR: Incorrect parameters. Try HDBackup.exe /h for help." << endl;
		out = 1;
	}
	return out;
}

int backup()
{

}