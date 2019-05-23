# endiffo - Environment Diff Tool
Endiffo is an environment comparison tool. It will allow you to:
- Record a subset of the system state, focusing on commonly-used locations where configuration is kept as well as user-specified locations.
- Compare snapshots to discover differences
- Apply snapshots (providing a summary of changes to be made and a big warning before continuing)

It may also prove useful to run the application in VMs, Docker containers, at different times on the same machine or between dev and test environments:

It will be possible to make the file as small or comprehensive as required, and perform a diff between saved snapshots.

Endiffo runs on both Windows and Linux.

# Command Line Arguments
-o --output Specify output filename. By default this is “snapshot_DATETIME.endiffo”, where DATETIME is an ISO 8601-formatted date.
