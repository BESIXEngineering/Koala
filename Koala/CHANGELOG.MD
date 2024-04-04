## [Unreleased]
### Added
### Changed
### Deprecated
### Removed
### Fixed
### Security

## 3.14
### Added
- Member1D BulcklingGroup input parameter
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
