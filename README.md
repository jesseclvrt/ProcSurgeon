# ProcSurgeon

Ever try capturing when a process starts in ProcMon, only to be overwhelmed with 200,000 entries a second? Tired of clicking the button to start logging then frantically starting the process ASAP? Me too. That's why I wrote this small tool as a wrapper for ProcMon. This will help you get a log file cleanly cut to the moments you're actually interested about.

## Installation

You will need the ProcMon executable along side this executable. You may download this as a release.

## Usage

-p, --procmon... Path of the procmon executable

-t, --target... Path of the target executable

-o, --output... Path of the output

-m, --millis[optional]... How many milliseconds to log after starting the target. If you do not include this parameter, ProcSurgeon will wait for the target process to exit.


## Author

Hastily written in a couple hours by Jesse Calvert