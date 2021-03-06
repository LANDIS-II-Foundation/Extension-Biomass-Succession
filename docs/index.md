# What is the Biomass Succession Extension?

Biomass succession simulates changes in the biomass of each cohort dependent upon age, competition, and disturbance. The extension has been a popular tool when the focus of the research is on aboveground forest dynamics and when consideration of below ground dynamics is not important. Relative to succession extensions that track all possible Carbon pools (NECN Succession and Forest Carbon Succession), it is relatively simple. 

The Biomass Succession Extension generally follows the methods outlined in Scheller and Mladenoff (2004). Biomass Succession calculates how cohorts reproduce, age, and die and how cohort biomass (g m-2) changes through time. The Biomass Succession extension also tracks dead biomass over time, divided into two pools: woody and leaf litter. 

Versions 2 and 3 of the Biomass Succession extension is conceptually nearly identical to version 1.x, although the inputs have changed. Notably, the user can now specify the probability of establishment given the species shade tolerance and site shade. Also, the user now specifies the maximum biomass by species and ecoregion, allowing better representation of shrubs and grasses. Leaf litter decay rates are no longer input and are a function of species leaf lignin and ecoregion actual evapotranspiration. 

# Features

- [x] Relatively few parameters organized by ecoregions.
- [x] Provides optional initiation of the Climate Library, for incorporating monthly or daily climate forecasts.

# Release Notes

- Latest official release: Version 5.3.1 — May 2021
- [Biomass Succession User Guide](https://github.com/LANDIS-II-Foundation/Extension-Biomass-Succession/blob/master/docs/LANDIS-II%20Biomass%20Succession%20v5%20User%20Guide.pdf).
- [User Guide for Climate Library](https://github.com/LANDIS-II-Foundation/Library-Climate/blob/master/docs/LANDIS-II%20Climate%20Library%20v4.2%20User%20Guide.pdf)
- Full release details found in the NECN User Guide and on GitHub.

# Requirements

To use Biomass Succession, you need:

- The [LANDIS-II model v7.0](http://www.landis-ii.org/install) installed on your computer.
- Example files (see below)

# Download

The latest version can be downloaded [here](https://github.com/LANDIS-II-Foundation/Extension-Biomass-Succession/blob/master/deploy/installer/LANDIS-II-V7%20Biomass%20Succession%205.3.1-setup.exe). To install it on your computer, just launch the installer.

# Example Files

LANDIS-II requires a global parameter file for your scenario, and then different parameter files for each extension.

Example files are [here](https://downgit.github.io/#/home?url=https://github.com/LANDIS-II-Foundation/Extension-Biomass-Succession/blob/master/testings/CoreV7.0-BiomassSuccession5.3).

# Citation

Scheller, R.M. and D.J. Mladenoff. 2004. A forest growth and biomass module for a landscape simulation model, LANDIS: Design, validation, and application. Ecological Modelling 180: 211-229. 

# Support

If you have a question, please contact Robert Scheller. 
You can also ask for help in the [LANDIS-II users group](http://www.landis-ii.org/users).

If you come across any issue or suspected bug, please post about it in the [issue section of the Github repository](https://github.com/LANDIS-II-Foundation/Extension-Biomass-Succession/issues) (GitID required).

# Author

[The LANDIS-II Foundation](http://www.landis-ii.org)

Mail : rschell@ncsu.edu
