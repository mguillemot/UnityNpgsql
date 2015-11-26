# Postgresql for Unity3d

This is a fork of [Npgsql](https://github.com/npgsql/npgsql) designed to work with the [Unity3d](http://unity3d.com/) engine. It tracks the latest 2.x release. 3.x will never be supported since it relies on "modern" .NET features that are not available in the old Mono runtime of Unity3d.

- **Based on Npgsql 2.2.7**
- Remove support for internationalization.
- Remove support for HTTPS.
- Move to the `UnityNpgsql` namespace to prevent some nasty conflict with the old `Npgsql.dll` used in the Editor domain.

# Usage

Simply copy the proper `UnityNpgsql.dll` assembly (Debug or Release) to the *Assets/Plugin/* folder in your project.