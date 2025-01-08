﻿using System;
using System.DirectoryServices;
using System.Runtime.InteropServices;

namespace Frends.HTTP.Request.Tests;

internal class Helper
{
    public static string CreateTestUser(string domain, string name, string pwd)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new PlatformNotSupportedException("UseGivenCredentials feature is only supported on Windows.");

        using DirectoryEntry AD = new DirectoryEntry("WinNT://" + domain + ",computer");
        DirectoryEntry NewUser = AD.Children.Add(name, "user");
        NewUser.Invoke("SetPassword", new object[] { pwd });
        NewUser.Invoke("Put", new object[] { "Description", "Test User from .NET" });
        NewUser.CommitChanges();
        DirectoryEntry grp;

        grp = AD.Children.Find("Administrators", "group");
        if (grp != null)
            grp.Invoke("Add", new object[] { NewUser.Path.ToString() });
        return $"{domain}//{name}";
    }

    public static void DeleteTestUser(string name)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new PlatformNotSupportedException("UseGivenCredentials feature is only supported on Windows.");

        using DirectoryEntry localDirectory = new DirectoryEntry($"WinNT://{Environment.MachineName}");
        DirectoryEntries users = localDirectory.Children;
        DirectoryEntry user = users.Find(name);
        users.Remove(user);
    }
}
