
# Ironman Mark 3 Suit

This repository was created to contain the code used to make my Ironman Mark 3 costume function. Whilst it looks like there were not many commits, please believe me that there were countless other throw away projects that I made use of before ending up with this pretty distilled setup for my code.

Pictures of the suit can be found at this here: 
[![Static Badge](https://img.shields.io/badge/Instagram-%23FFFFFF?logo=instagram)](https://www.instagram.com/reel/DBj9iL3IK-m)

There are a number of projects in the solution, aimed at targeted parts of t he suit. It did result in some of the builds being much quicker than others and also smaller in size.
I do have some parts that need to be added to complete the codebase for the suit but I was feeling burnt out after 7 months of working on the full build whilst also working full time, have other hobbies.... and try to be a somewhat present husband.

## Where to get the 3d files for the Suit
Whilst I am not willing to put the files for the suit in this repo for distribution (because I didnt make them). They can be found over at Wireframe 3d https://www.wf3d.shop/collections/marvel/products/ironman-mark-3-suit-3d-printable-model

## Whats missing
The code for running the arc reactor has intentionally been missed out because whilst it was functional, I wanted to work out a different way to do the animations that were not so clunky, once I have worked out how to make the animations work much more smoothly I will add those into the main codebase.

The code for running the calf flaps were also never written because I didnt like the idea of either stuffing lipo batteries next to my calf muscles and then knocking something or damaging them somehow, and I also didnt like the idea of running cables from somewhere else in the suit to the calf servos.

## Required components
- https://www.amazon.co.uk/dp/B07DPSMRJ6?ref=ppx_yo2ov_dt_b_fed_asin_title&th=1 - push buttons
- https://www.amazon.co.uk/dp/B08BZGC22Q?ref=ppx_yo2ov_dt_b_fed_asin_title&th=1 - unsoldered esp32 wroom-32
- https://www.amazon.co.uk/dp/B0C1N8VZ1S?ref=ppx_yo2ov_dt_b_fed_asin_title&th=1 - MF90 servos
- https://www.amazon.co.uk/dp/B093LFBWTL?ref=ppx_yo2ov_dt_b_fed_asin_title - MG90S servos
- https://www.amazon.co.uk/dp/B08V11Z88Q?ref=ppx_yo2ov_dt_b_fed_asin_title - 3K mAh LIPO batteries (for all other parts)
- https://www.amazon.co.uk/dp/B0DB1VKT9X?ref=ppx_yo2ov_dt_b_fed_asin_title - 4k mAh LIPO batteries (for the back)
- https://www.amazon.co.uk/dp/B09STS5YZX?ref=ppx_yo2ov_dt_b_fed_asin_title  - Spring loaded magnetic pogo connectors
- https://www.amazon.co.uk/dp/B08BYSWZBT?ref=ppx_yo2ov_dt_b_fed_asin_title - 7 pixel neo pixel type board
- https://www.amazon.co.uk/dp/B07V1VZPX6?ref=ppx_yo2ov_dt_b_fed_asin_title&th=1 - 12 pixel neo pixel ring
- https://www.amazon.co.uk/gp/product/B08977C9YM/ref=ppx_yo_dt_b_search_asin_title?ie=UTF8&psc=1 - Step down converter with digital voltage display
- https://www.amazon.co.uk/AZDelivery-MT3608-LM2596S-Downpower-Arduino/dp/B07DP3JX2X?th=1 - Stepdown converter without digital voltage display
- https://www.amazon.co.uk/gp/product/B06XCW26LG/ref=ppx_yo_dt_b_search_asin_title?ie=UTF8&psc=1 - MT3608 Step up power converter
- https://www.amazon.co.uk/gp/product/B008R50AA0/ref=ppx_yo_dt_b_search_asin_title?ie=UTF8&psc=1 - slide switch
- https://uk.robotshop.com//products/ai-thinker-nodemcu-vc-02-kit-offline-speech-recognition-module - Offline voice recognition module
- https://www.amazon.co.uk/gp/product/B075QCPNBP/ref=ppx_yo_dt_b_search_asin_title?ie=UTF8&psc=1 - RCWL-0516 Microwave Radar Motion Sensor
- https://www.amazon.co.uk/gp/product/B07P5YXBXV/ref=ppx_yo_dt_b_search_asin_title?ie=UTF8&psc=1 - MPU-6050 6DOF 3 Axis Gyroscope+Accelerometer Module

## What is covered

# Left Gauntlet

- 1 X Push button
- 2 X 3.7V 3K mAh LIPO battery
- 1 X ESP32-WROOM 
- 1 X 7 Pixel neopixel type board
- 1 X LM2596S Step down converter
- 1 X Slide switch
- 3 X MG90S Servos

# Right Gauntlet

- 1 X Push button
- 2 X 3.7V 3K mAh LIPO battery
- 1 X ESP32-WROOM 
- 1 X 7 Pixel neopixel type board
- 1 X LM2596S Step down converter
- 1 X Slide switch
- 3 X MG90S Servos

# Helmet

- 1 X 3.7V 3K mAh LIPO battery
- 1 X ESP32-WROOM
- 1 X Slide switch
- 2 X MG90S Servos
- 1 X AI Thinker Offline Voice Recognition Module
- 1 X MT3608 Step up power converter

# Back, Shoulder, and Clavicle

- 1 X ESP32-WROOM
- 1 X Slide switch
- 2 X 3.7V 4K mAh LIPO battieres
- 6 X MG90S Servos
- 6 X MF90 Servos
- 1 X RCWL-0516 Microwave Radar Motion Sensor
- 1 X MPU-6050 6DOF 3 Axis Gyroscope+Accelerometer Module
- 1 X Step down converter with digital voltage display
