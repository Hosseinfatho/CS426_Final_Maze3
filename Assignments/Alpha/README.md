# Alpha Release Documentation

[Link to GitHub Repository](https://github.com/gradyl16/maze3)

## Improvements & Deliverables

### UI Design

- Applied a contrasting texture to the walls to create a clear distinction between them and the floor
  - This maintains the computer-based theme while removing unnecessary player frustration due to similar looking physical structures
- Reduced the intensity of character spawn spotlights
  - Still brings attention to NPC and character start points without being unnecessarily saturated
- Modified textures to match similar structures so unnecessary attention is not brought onto uninteresting structures
- Flickering light added to area with master enemy
- Spotlight saturation reduced for enemy spawn points

### Sound Design

- Added a death sound for the ground-based enemies
- Added a short digital audio blip upon cube face traversal
- Added lowkey background sound to set the tone as a digitized environment
  - Note: we did not include any additional background sound as we could not figure out how to integrate it with the existing sound without the sounds contradicting and fighting each other, ruining the audio quality. We would be happy to fix this if there are specific suggestions on how we might benefit from additional background sound.
- Added alarm sound once a player nears a strong enemy
- Added ominous background music while standing near the edge of one face; helps set the tone for the next "level" having a more substantial challenge ahead