# NOTE:  THESE INSTRUCTIONS ARE FOR THE UPCOMING v8 RELEASE.

# What is the Biomass Succession Extension?

Biomass succession simulates changes in the biomass of each cohort dependent upon age, competition, and disturbance. The extension has been a popular tool when the focus of the research is on aboveground forest dynamics and when consideration of below ground dynamics is not important. Relative to succession extensions that track all possible Carbon pools (NECN Succession and Forest Carbon Succession), it is relatively simple. 

The Biomass Succession Extension generally follows the methods outlined in Scheller and Mladenoff (2004). Biomass Succession calculates how cohorts reproduce, age, and die and how cohort biomass (g m-2) changes through time. The Biomass Succession extension also tracks dead biomass over time, divided into two pools: woody and leaf litter. 

Versions 2 and later of the Biomass Succession extension is conceptually nearly identical to version 1.x, although the inputs have changed. Notably, the user can now specify the probability of establishment given the species shade tolerance and site shade. Later versions largely tracked major Core releases.  

# Features

- [x] Relatively few parameters organized by ecoregions.
- [x] Provides optional initiation of the Climate Library, for incorporating monthly or daily climate forecasts.

# Release Notes

- Latest official release: Version 7.0.0 — August 2024
- [Biomass Succession User Guide](https://github.com/LANDIS-II-Foundation/Extension-Biomass-Succession/blob/master/docs/LANDIS-II%20Biomass%20Succession%20v7%20User%20Guide.pdf).
- [User Guide for Climate Library](https://github.com/LANDIS-II-Foundation/Library-Climate/blob/master/docs/LANDIS-II%20Climate%20Library%20v4.2%20User%20Guide.pdf)
- Full release details found in the NECN User Guide and on GitHub.

# Requirements

To use Biomass Succession, you need:

- The [LANDIS-II model v8.0](http://www.landis-ii.org/install) installed on your computer.
- Example files (see below)

# Download the Extension

The latest version can be downloaded [here](https://github.com/LANDIS-II-Foundation/Extension-Biomass-Succession/blob/master/deploy/installer/LANDIS-II-V8%20Biomass%20Succession%207.0.0-setup.exe). (Look for the download icon |_| in the upper right.) To install it on your computer, launch the installer.

# Citation

Scheller, R. M. and Mladenoff, D. J. A forest growth and biomass module for a landscape simulation model, LANDIS: Design, validation, and application. Ecological Modelling. 2004; 180(1):211-229. 

# Example Files

LANDIS-II requires a global parameter file for your scenario, and then different parameter files for each extension.

Example files are [here](https://downgit.github.io/#/home?url=https://github.com/LANDIS-II-Foundation/Extension-Biomass-Succession/blob/master/testings/CoreV8.0-BiomassSuccession7.0).

# Support

If you have a question, please contact Robert Scheller. 
You can also ask for help in the [LANDIS-II users group](http://www.landis-ii.org/users).

If you come across any issue or suspected bug, please post about it in the [issue section of the Github repository](https://github.com/LANDIS-II-Foundation/Extension-Biomass-Succession/issues) (GitID required).

# Author

[The LANDIS-II Foundation](http://www.landis-ii.org)

Mail : rschell@ncsu.edu
