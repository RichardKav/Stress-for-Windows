# Stress-for-Windows
A tool for stressing Windows hosts in a similar fashion to the Linux tool Stress.

This tool aims to stress a cpu at a fixed level on Windows based machines. It is useful for testing output such as power consumption on a Windows Machine. Its usage is:

It takes 3 parameters:

1) cpu usage as a percentage
2) the count of cores to perform the stress test on. The command all can be used instead.
3) the duration in seconds for which this should run for.

i.e. WindowsStress.exe 50 all 20

meaning stress to 50% usage for all cores for 20 seconds.
