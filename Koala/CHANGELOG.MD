## [Unreleased]
### Added
### Changed
### Deprecated
### Removed
### Fixed
### Security

## 3.25.2 (2026-06-10)
### Fixed
- Write LoadGroup objects before LoadCase objects in XML, so that LoadCases can reference the correct LoadGroups.

## 3.25.1 (2026-05-07)
### Fixed
- LoadCase without ModificationGroup no longer gets assigned a "None" p6 element (Modification group) value in the XML file, as this can occasionally cause SCIA load issues.

## 3.25.0 (2026-05-04)
### Added
- Support for Numerical Cross Section definitions
- Support for Steel Cross Section type SheetWelded I, symmetric (IWES)
- Html help with examples for Cross-Sections component
- Model Modifier: Added AbsentElement component to define an absent beam, slab or support
- Added ModificationGroup input parameter to LoadCase component
	NOTE: will give an error when opening scripts that use a LoadCase component without the new parameter. This error can be safely ignored.

## 3.24.0 (2026-01-07)
### Added
- KoalaConvert component providing a simple conversion mechanism for any object that has implemented a ToKoala method returning a List(Of Dictionary(Of String, String)). Each Dictionary in the List should hold the required object properties of a single Koala object.
- Add support for JSON formatted material strings
### Changed
- CreateXML component's auto-detect parameters feature also searches for Koala components nested inside Grasshopper clusters.

## 3.23.1 (2025-02-07)
### Added
- ISEC profile type added to recognised section types
### Fixed
- Fixed Enum error (duplicate Enum value) in LoadCaseType

## 3.23.0 (2025-01-17)
### Added
- New FreeSurfaceLoad component with possibility to input 3 load values (for 3 pt distribution type)

## 3.22.2 (2024-11-26)
### Fixed
- Fixed RigidArm reference to duplicate nodes

## 3.22.1 (2024-10-23)
### Fixed
- NonLinear Function Positive/Negative End Definition
- LineHinge Tx|Ty|Tz|Rx input parameter names
- FreePointLoad & FreeMomentLoad validity and system parameters

## 3.22.0 (2024-09-18)
### Added
- Added Material input to CreateXML component to modify steel and concrete material properties of materials already existing in the esa file
### Changed
- Enabled parsing Enum Parameter values from its string representation
- LoadCase and LoadGroup components: Modified input parameters to Enum type
- LoadGroup component: Reduced amount of output values to 3 (corresponding to the LoadCases&LoadGroups component) for backward compatibility

## 3.21 (2024-09-12)
### Changed
- Added LoadGroup VariableLoadType input to LoadGroup component

## 3.20 (2024-08-30)
### Added
- ResultClasses component

## 3.19 (2024-08-27)
### Added
- LoadGroups component (replaces original LoadCases&LoadGroups component)
- LoadCases component (replaces original LoadCases&LoadGroups component)
- SeismicSpectrums component
### Deprecated
- LoadCases&LoadGroups component

## 3.18 (2024-08-26)
### Added
- Added functionality for Accidental and Seismic Load Groups

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
