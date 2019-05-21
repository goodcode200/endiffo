# endiffo
Endiffo is an environment comparison tool for Linux and Windows. It should make it easier to spot that one difference between dev and test, or compare your settings now to those you had 5 days ago. It works by making a snapshot of common and user-specified locations where configuration is stored, for example your environment variables and hosts file. The information can be stored long-term and/or transferred between systems for comparison.

It will be possible to make the file as small or comprehensive as required, and perform a diff between saved snapshots.

# Command Line Arguments
-o	Specify output filename. By default this is “snapshot_DATETIME.endiffo”, where DATETIME is an ISO 8601-formatted date.
