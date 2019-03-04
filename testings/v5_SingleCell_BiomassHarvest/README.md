# Single Cell Biomass Harvest

This test was designed to demonstrate whether partial harvest and material left on site reflected intentions.

Demonstration outputs:
Biomass-succession-log-10harvest-0remove.csv
Biomass-succession-log-90harvest-0remove.csv
Biomass-succession-log-90harvest-50remove.csv
Biomass-succession-log-90harvest-100remove.csv

N-harvest:  The amount of partial harvest, 10% or 90%
N-remove:  The amound of cohort wood removed from the site, 0%, 50%, 100%

Some notes:
* Wood decay happens every 10 years which can create some odd-looking behavior when harvest is happening on a separate time step.
* I set the Biomass Harvest table so that 100% of DWD was removed at the beginning of a harvest.  This makes the amount left on site (the inverse of wood removed) more obvious.