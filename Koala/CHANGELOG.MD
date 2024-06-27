## [Unreleased]
### Added
### Changed
### Deprecated
### Removed
### Fixed
### Security

## 3.17 (2024-06-27)
### Fixed
- Reading scale and tolerance from ProjectData

## 3.16 (2024-06-17)
### Added
- Added SectionOn2D
### Changed
- Renamed SectionOnBeam to SectionOn1D

## 3.15 (2024-04-15)
### Fixed
- Assignment of NonLinear function to subsoil by moving NonLinear function definitions to the top of the XML file

## 3.14 (2024-04-04)
### Added
- Member1D BucklingGroup input parameter
- HingeOnBeam with NonLinear function parameters and NamePrefix (to replace Hinges component)
- Support components: added input parameters for name prefix and system definition
- Selection component
### Changed
- Restructured HelperTools and CreateXML methods
- Amount of values returned for a Member1D increased to 14
- Amount of values returned for a NodeSupport increased to 22
- Amount of values returned for a BeamPointSupport increased to 26
- Amount of values returned for a BeamLineSupport increased to 25
- Amount of values returned for a SurfaceEdgeSupport increased to 27
- Amount of values returned for a SurfaceSupport increased to 3
- Amount of values returned for a HingeOnBeam increased to 21
### Deprecated
- Hinges component
### Removed
- Obscured InternalNodeOnMember1D component as member/node connection is broken in the SCIA XML update
### Fixed
- Auto-remove feature for duplicate internal nodes
