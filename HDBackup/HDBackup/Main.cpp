#define VERSION "1.0"
#include <iostream>
using std::cout;
using std::cin;
using std::endl;

#include <string>
using std::string;
using std::to_string;
#include <sstream>
using std::getline;
#include <ctime>
using std::time;

#include <Windows.h>

string createTimestamp(tm t);
int runBackup(string dest, string src);

int main(int argc, char *argv[])
{
	int out = 0;
	string _source;
	string _dest;
	string _id;
	string timestamp;
	string mkdir;

	time_t t = time(0);
	struct tm now;
	localtime_s(&now, &t);
	timestamp = createTimestamp(now);

	if (argc == 1)
	{
		// User input
		cout << "User name and PC number: ";
		getline(cin, _id);
		cout << "Source drive letter: ";
		getline(cin, _source);
		// End user input
		out = runBackup(_dest + ":\\Backups\\" + _id, _source);
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
		out = runBackup(_dest + ":\\Backups\\" + argv[1], argv[2]);
	}
	else
	{
		cout << "ERROR: Incorrect parameters. Try HDBackup.exe /h for help." << endl;
		out = 1;
	}
	return out;
}

string createTimestamp(tm t)
{
	string out;
	out = to_string(t.tm_mon) + "/" + 
		  to_string(t.tm_mday) + "/" + 
		  to_string(t.tm_year) + "_" + 
		  to_string(t.tm_hour) + ":" + 
		  to_string(t.tm_min) + ":" + to_string(t.tm_sec);
	return out;
}

int runBackup(string dest, string src)
{
	int out = 0;

	CreateDirectory(dest.c_str(), NULL);

	return out;
}
