using MudBlazor;
using MudBlazor.Utilities;

namespace Cervantes.Web.Components.Layout;

public class CervantesTheme
{
    public static MudTheme Theme
    {
        get
        {
            return new MudTheme
            {
                /*Shadows = new Shadow
                {
                    Elevation = new string[] {
                        "none",
                            "0 5px 5px -3px rgba(0,0,0,.06), 0 8px 10px 1px rgba(0,0,0,.042), 0 3px 14px 2px rgba(0,0,0,.036)",
                            "0px 3px 1px -2px rgba(0,0,0,0.2),0px 2px 2px 0px rgba(0,0,0,0.14),0px 1px 5px 0px rgba(0,0,0,0.12)",
                            "0px 3px 3px -2px rgba(0,0,0,0.2),0px 3px 4px 0px rgba(0,0,0,0.14),0px 1px 8px 0px rgba(0,0,0,0.12)",
                            "0px 2px 4px -1px rgba(0,0,0,0.2),0px 4px 5px 0px rgba(0,0,0,0.14),0px 1px 10px 0px rgba(0,0,0,0.12)",
                            "0px 3px 5px -1px rgba(0,0,0,0.2),0px 5px 8px 0px rgba(0,0,0,0.14),0px 1px 14px 0px rgba(0,0,0,0.12)",
                            "0px 3px 5px -1px rgba(0,0,0,0.2),0px 6px 10px 0px rgba(0,0,0,0.14),0px 1px 18px 0px rgba(0,0,0,0.12)",
                            "0px 4px 5px -2px rgba(0,0,0,0.2),0px 7px 10px 1px rgba(0,0,0,0.14),0px 2px 16px 1px rgba(0,0,0,0.12)",
                            "0px 5px 5px -3px rgba(0,0,0,0.2),0px 8px 10px 1px rgba(0,0,0,0.14),0px 3px 14px 2px rgba(0,0,0,0.12)",
                            "0px 5px 6px -3px rgba(0,0,0,0.2),0px 9px 12px 1px rgba(0,0,0,0.14),0px 3px 16px 2px rgba(0,0,0,0.12)",
                            "0px 6px 6px -3px rgba(0,0,0,0.2),0px 10px 14px 1px rgba(0,0,0,0.14),0px 4px 18px 3px rgba(0,0,0,0.12)",
                            "0px 6px 7px -4px rgba(0,0,0,0.2),0px 11px 15px 1px rgba(0,0,0,0.14),0px 4px 20px 3px rgba(0,0,0,0.12)",
                            "0px 7px 8px -4px rgba(0,0,0,0.2),0px 12px 17px 2px rgba(0,0,0,0.14),0px 5px 22px 4px rgba(0,0,0,0.12)",
                            "0px 7px 8px -4px rgba(0,0,0,0.2),0px 13px 19px 2px rgba(0,0,0,0.14),0px 5px 24px 4px rgba(0,0,0,0.12)",
                            "0px 7px 9px -4px rgba(0,0,0,0.2),0px 14px 21px 2px rgba(0,0,0,0.14),0px 5px 26px 4px rgba(0,0,0,0.12)",
                            "0px 8px 9px -5px rgba(0,0,0,0.2),0px 15px 22px 2px rgba(0,0,0,0.14),0px 6px 28px 5px rgba(0,0,0,0.12)",
                            "0px 8px 10px -5px rgba(0,0,0,0.2),0px 16px 24px 2px rgba(0,0,0,0.14),0px 6px 30px 5px rgba(0,0,0,0.12)",
                            "0px 8px 11px -5px rgba(0,0,0,0.2),0px 17px 26px 2px rgba(0,0,0,0.14),0px 6px 32px 5px rgba(0,0,0,0.12)",
                            "0px 9px 11px -5px rgba(0,0,0,0.2),0px 18px 28px 2px rgba(0,0,0,0.14),0px 7px 34px 6px rgba(0,0,0,0.12)",
                            "0px 9px 12px -6px rgba(0,0,0,0.2),0px 19px 29px 2px rgba(0,0,0,0.14),0px 7px 36px 6px rgba(0,0,0,0.12)",
                            "0px 10px 13px -6px rgba(0,0,0,0.2),0px 20px 31px 3px rgba(0,0,0,0.14),0px 8px 38px 7px rgba(0,0,0,0.12)",
                            "0px 10px 13px -6px rgba(0,0,0,0.2),0px 21px 33px 3px rgba(0,0,0,0.14),0px 8px 40px 7px rgba(0,0,0,0.12)",
                            "0px 10px 14px -6px rgba(0,0,0,0.2),0px 22px 35px 3px rgba(0,0,0,0.14),0px 8px 42px 7px rgba(0,0,0,0.12)",
                            "0px 11px 14px -7px rgba(0,0,0,0.2),0px 23px 36px 3px rgba(0,0,0,0.14),0px 9px 44px 8px rgba(0,0,0,0.12)",
                            "0px 11px 15px -7px rgba(0,0,0,0.2),0px 24px 38px 3px rgba(0,0,0,0.14),0px 9px 46px 8px rgba(0,0,0,0.12)",
                            "0 5px 5px -3px rgba(0,0,0,.06), 0 8px 10px 1px rgba(0,0,0,.042), 0 3px 14px 2px rgba(0,0,0,.036)" }
                },
                PaletteLight = new MudBlazor.PaletteLight
                {
                    AppbarBackground = new MudBlazor.Utilities.MudColor("#ffffff"),
                    Primary = new MudBlazor.Utilities.MudColor("#3459e6"),
                    Secondary = new MudBlazor.Utilities.MudColor("#ff4081"),
                    Tertiary = new MudBlazor.Utilities.MudColor("#ffffffb3"),
                    Info = new MudBlazor.Utilities.MudColor("#287bb5"),
                    Success = new MudBlazor.Utilities.MudColor("#2fb380"),
                    Warning = new MudBlazor.Utilities.MudColor("#EE9B00"),
                    Error = new MudBlazor.Utilities.MudColor("#AE2012"),
                    Dark = new MudBlazor.Utilities.MudColor("#27272fff"),
                    AppbarText = new MudBlazor.Utilities.MudColor("#424242"),
                    DrawerText = new MudBlazor.Utilities.MudColor("#ffffffb3"),
                    DrawerIcon = new MudBlazor.Utilities.MudColor("#ffffffb3"),
                },
                PaletteDark = new MudBlazor.PaletteDark
                {
                    AppbarBackground = new MudBlazor.Utilities.MudColor("#32333d"),
                    AppbarText = new MudBlazor.Utilities.MudColor("#ffffffb3"),
                    Primary = new MudBlazor.Utilities.MudColor("#3459e6"),
                    Secondary = new MudBlazor.Utilities.MudColor("#ff4081"),
                    Tertiary = new MudBlazor.Utilities.MudColor("#ffffffb3"),
                    Info = new MudBlazor.Utilities.MudColor("#287bb5"),
                    Success = new MudBlazor.Utilities.MudColor("#2fb380"),
                    Warning = new MudBlazor.Utilities.MudColor("#EE9B00"),
                    Error = new MudBlazor.Utilities.MudColor("#AE2012"),
                    Dark = new MudBlazor.Utilities.MudColor("#27272fff"),
                    DrawerText = new MudBlazor.Utilities.MudColor("#ffffffb3"),
                    DrawerIcon = new MudBlazor.Utilities.MudColor("#ffffffb3"),
                }*/
                
                PaletteLight = new PaletteLight()
                {
                    // Indigo colors
                    Primary = "#4F46E5",       // Indigo-600
                    PrimaryDarken = "#4338CA", // Indigo-700
                    PrimaryLighten = "#6366F1", // Indigo-500
                    
                    // Neutral colors for a clean look
                    Background = "#fafafa",
                    Surface = "#F9FAFB",
                    DrawerBackground = "#fafafa",
                    DrawerText = "#1F2937",
                    
                    // Text colors
                    TextPrimary = "#1F2937",
                    TextSecondary = "#6B7280",
                    
                    // Other components
                    AppbarBackground = "#fafafa",
                    AppbarText = "#1F2937",
                    
                    // Action colors
                    Info = "#60A5FA",   // Blue-400
                    Success = "#10B981", // Emerald-500
                    Warning = "#F59E0B", // Amber-500
                    Error = "#EF4444",   // Red-500
                    
                    // Customizing other elements
                    ActionDefault = "#6B7280",
                    ActionDisabled = "#E5E7EB",
                    ActionDisabledBackground = "#F3F4F6",
                    
                    // Dividers and lines
                    LinesDefault = "#E5E7EB",
                    LinesInputs = "#D1D5DB",
                    
                    // Tables
                    TableLines = "#E5E7EB",
                    TableStriped = "#F9FAFB",
                    TableHover = "#F3F4F6",
                },
                PaletteDark = new PaletteDark()
                {
                    // Dark mode indigo colors
                    Primary = "#6366F1",     // Indigo-500
                    PrimaryDarken = "#4F46E5", // Indigo-600
                    PrimaryLighten = "#818CF8", // Indigo-400
                    
                    // Dark mode neutrals
                    Background = "#111827",  // Gray-900
                    Surface = "#1F2937",     // Gray-800
                    DrawerBackground = "#111827",
                    DrawerText = "#F9FAFB",
                    
                    // Dark mode text
                    TextPrimary = "#F9FAFB",
                    TextSecondary = "#9CA3AF",
                    
                    // Other colors
                    AppbarBackground = "#111827",
                    AppbarText = "#F9FAFB",
                    
                    // Action colors
                    Info = "#3B82F6",
                    Success = "#10B981",
                    Warning = "#F59E0B",
                    Error = "#EF4444",
                    
                    // Customizing other elements
                    ActionDefault = "#9CA3AF",
                    ActionDisabled = "#374151",
                    ActionDisabledBackground = "#1F2937",
                    
                    // Dividers and lines
                    LinesDefault = "#374151",
                    LinesInputs = "#4B5563",
                    
                    // Tables
                    TableLines = "#374151",
                    TableStriped = "#1F2937",
                    TableHover = "#263244",
                },
                      Typography = new Typography
                     {
                         Default = new DefaultTypography()
                         {
                             FontFamily = new[] { "Inter", "ui-sans-serif", "system-ui", "sans-serif" },
                             FontSize = "0.875rem",
                             FontWeight = "400",
                             LineHeight = "1.5",
                             LetterSpacing = "normal"
                         },
                         H1 = new H1Typography()
                         {
                             FontFamily = new[] { "Inter", "ui-sans-serif", "system-ui", "sans-serif" },
                             FontSize = "2.25rem",
                             FontWeight = "700",
                             LineHeight = "1.2",
                             LetterSpacing = "-0.025em"
                         },
                         H2 = new H2Typography()
                         {
                             FontFamily = new[] { "Inter", "ui-sans-serif", "system-ui", "sans-serif" },
                             FontSize = "1.875rem",
                             FontWeight = "700",
                             LineHeight = "1.2",
                             LetterSpacing = "-0.025em"
                         },
                         H3 = new H3Typography()
                         {
                             FontFamily = new[] { "Inter", "ui-sans-serif", "system-ui", "sans-serif" },
                             FontSize = "1.5rem",
                             FontWeight = "600",
                             LineHeight = "1.2",
                             LetterSpacing = "-0.025em"
                         },
                         H4 = new H4Typography()
                         {
                             FontFamily = new[] { "Inter", "ui-sans-serif", "system-ui", "sans-serif" },
                             FontSize = "1.25rem",
                             FontWeight = "600",
                             LineHeight = "1.2"
                         },
                         H5 = new H5Typography()
                         {
                             FontFamily = new[] { "Inter", "ui-sans-serif", "system-ui", "sans-serif" },
                             FontSize = "1.125rem",
                             FontWeight = "600",
                             LineHeight = "1.2"
                         },
                         H6 = new H6Typography()
                         {
                             FontFamily = new[] { "Inter", "ui-sans-serif", "system-ui", "sans-serif" },
                             FontSize = "1rem",
                             FontWeight = "600",
                             LineHeight = "1.2"
                         },
                         Body1 = new Body1Typography()
                         {
                             FontFamily = new[] { "Inter", "ui-sans-serif", "system-ui", "sans-serif" },
                             FontSize = "0.875rem",
                             FontWeight = "400",
                             LineHeight = "1.5"
                         },
                         Button = new ButtonTypography()
                         {
                             FontFamily = new[] { "Inter", "ui-sans-serif", "system-ui", "sans-serif" },
                             FontSize = "0.875rem",
                             FontWeight = "500",
                             LineHeight = "1.75",
                             TextTransform = "none"
                         }
                     },
                     LayoutProperties = new LayoutProperties
                     {
                         DefaultBorderRadius = "0.5rem",          // Matches your --radius
                         DrawerWidthLeft = "260px",
                         DrawerWidthRight = "260px"
                     },
                Shadows = new Shadow()
                {
                    // Subtle shadows for a modern look
                  /*Elevation = new string[]
                   {
                       "none", // 0
                       "0 1px 2px rgba(0,0,0,0.04), 0 1px 1px rgba(0,0,0,0.03)", // 1 - Very subtle
                       "0 2px 4px -1px rgba(0,0,0,0.04), 0 1px 2px rgba(0,0,0,0.03)", // 2
                       "0 3px 5px -1px rgba(0,0,0,0.04), 0 2px 3px rgba(0,0,0,0.03)", // 3
                       "0 4px 6px -2px rgba(0,0,0,0.04), 0 2px 4px rgba(0,0,0,0.03)", // 4
                       "0 5px 8px -3px rgba(0,0,0,0.05), 0 3px 6px rgba(0,0,0,0.04)", // 5
                       "0 6px 10px -3px rgba(0,0,0,0.05), 0 3px 8px rgba(0,0,0,0.04)", // 6
                       "0 7px 12px -4px rgba(0,0,0,0.05), 0 4px 10px rgba(0,0,0,0.04)", // 7
                       "0 8px 14px -4px rgba(0,0,0,0.06), 0 5px 12px rgba(0,0,0,0.05)", // 8
                       "0 9px 16px -5px rgba(0,0,0,0.06), 0 5px 14px rgba(0,0,0,0.05)", // 9
                       "0 10px 18px -5px rgba(0,0,0,0.07), 0 6px 16px rgba(0,0,0,0.06)", // 10
                       "0 11px 20px -6px rgba(0,0,0,0.07), 0 7px 18px rgba(0,0,0,0.06)", // 11
                       "0 12px 22px -6px rgba(0,0,0,0.08), 0 8px 20px rgba(0,0,0,0.07)", // 12
                       "0 13px 24px -7px rgba(0,0,0,0.08), 0 9px 22px rgba(0,0,0,0.07)", // 13
                       "0 14px 26px -7px rgba(0,0,0,0.09), 0 10px 24px rgba(0,0,0,0.08)", // 14
                       "0 15px 28px -8px rgba(0,0,0,0.09), 0 11px 26px rgba(0,0,0,0.08)", // 15
                       "0 16px 30px -8px rgba(0,0,0,0.1), 0 12px 28px rgba(0,0,0,0.09)", // 16
                       "0 17px 32px -9px rgba(0,0,0,0.1), 0 13px 30px rgba(0,0,0,0.09)", // 17
                       "0 18px 34px -9px rgba(0,0,0,0.11), 0 14px 32px rgba(0,0,0,0.1)", // 18
                       "0 19px 36px -10px rgba(0,0,0,0.11), 0 15px 34px rgba(0,0,0,0.1)", // 19
                       "0 20px 38px -10px rgba(0,0,0,0.12), 0 16px 36px rgba(0,0,0,0.11)", // 20
                       "0 21px 40px -11px rgba(0,0,0,0.12), 0 17px 38px rgba(0,0,0,0.11)", // 21
                       "0 22px 42px -11px rgba(0,0,0,0.13), 0 18px 40px rgba(0,0,0,0.12)", // 22
                       "0 23px 44px -12px rgba(0,0,0,0.13), 0 19px 42px rgba(0,0,0,0.12)", // 23
                       "0 24px 46px -12px rgba(0,0,0,0.14), 0 20px 44px rgba(0,0,0,0.13)", // 24
                       "0 25px 48px -13px rgba(0,0,0,0.14), 0 20px 46px rgba(0,0,0,0.13)"  // 25
                   }*/
                  Elevation = new string[] {
                        "none",
                            "0 5px 5px -3px rgba(0,0,0,.06), 0 8px 10px 1px rgba(0,0,0,.042), 0 3px 14px 2px rgba(0,0,0,.036)",
                            "0px 3px 1px -2px rgba(0,0,0,0.2),0px 2px 2px 0px rgba(0,0,0,0.14),0px 1px 5px 0px rgba(0,0,0,0.12)",
                            "0px 3px 3px -2px rgba(0,0,0,0.2),0px 3px 4px 0px rgba(0,0,0,0.14),0px 1px 8px 0px rgba(0,0,0,0.12)",
                            "0px 2px 4px -1px rgba(0,0,0,0.2),0px 4px 5px 0px rgba(0,0,0,0.14),0px 1px 10px 0px rgba(0,0,0,0.12)",
                            "0px 3px 5px -1px rgba(0,0,0,0.2),0px 5px 8px 0px rgba(0,0,0,0.14),0px 1px 14px 0px rgba(0,0,0,0.12)",
                            "0px 3px 5px -1px rgba(0,0,0,0.2),0px 6px 10px 0px rgba(0,0,0,0.14),0px 1px 18px 0px rgba(0,0,0,0.12)",
                            "0px 4px 5px -2px rgba(0,0,0,0.2),0px 7px 10px 1px rgba(0,0,0,0.14),0px 2px 16px 1px rgba(0,0,0,0.12)",
                            "0px 5px 5px -3px rgba(0,0,0,0.2),0px 8px 10px 1px rgba(0,0,0,0.14),0px 3px 14px 2px rgba(0,0,0,0.12)",
                            "0px 5px 6px -3px rgba(0,0,0,0.2),0px 9px 12px 1px rgba(0,0,0,0.14),0px 3px 16px 2px rgba(0,0,0,0.12)",
                            "0px 6px 6px -3px rgba(0,0,0,0.2),0px 10px 14px 1px rgba(0,0,0,0.14),0px 4px 18px 3px rgba(0,0,0,0.12)",
                            "0px 6px 7px -4px rgba(0,0,0,0.2),0px 11px 15px 1px rgba(0,0,0,0.14),0px 4px 20px 3px rgba(0,0,0,0.12)",
                            "0px 7px 8px -4px rgba(0,0,0,0.2),0px 12px 17px 2px rgba(0,0,0,0.14),0px 5px 22px 4px rgba(0,0,0,0.12)",
                            "0px 7px 8px -4px rgba(0,0,0,0.2),0px 13px 19px 2px rgba(0,0,0,0.14),0px 5px 24px 4px rgba(0,0,0,0.12)",
                            "0px 7px 9px -4px rgba(0,0,0,0.2),0px 14px 21px 2px rgba(0,0,0,0.14),0px 5px 26px 4px rgba(0,0,0,0.12)",
                            "0px 8px 9px -5px rgba(0,0,0,0.2),0px 15px 22px 2px rgba(0,0,0,0.14),0px 6px 28px 5px rgba(0,0,0,0.12)",
                            "0px 8px 10px -5px rgba(0,0,0,0.2),0px 16px 24px 2px rgba(0,0,0,0.14),0px 6px 30px 5px rgba(0,0,0,0.12)",
                            "0px 8px 11px -5px rgba(0,0,0,0.2),0px 17px 26px 2px rgba(0,0,0,0.14),0px 6px 32px 5px rgba(0,0,0,0.12)",
                            "0px 9px 11px -5px rgba(0,0,0,0.2),0px 18px 28px 2px rgba(0,0,0,0.14),0px 7px 34px 6px rgba(0,0,0,0.12)",
                            "0px 9px 12px -6px rgba(0,0,0,0.2),0px 19px 29px 2px rgba(0,0,0,0.14),0px 7px 36px 6px rgba(0,0,0,0.12)",
                            "0px 10px 13px -6px rgba(0,0,0,0.2),0px 20px 31px 3px rgba(0,0,0,0.14),0px 8px 38px 7px rgba(0,0,0,0.12)",
                            "0px 10px 13px -6px rgba(0,0,0,0.2),0px 21px 33px 3px rgba(0,0,0,0.14),0px 8px 40px 7px rgba(0,0,0,0.12)",
                            "0px 10px 14px -6px rgba(0,0,0,0.2),0px 22px 35px 3px rgba(0,0,0,0.14),0px 8px 42px 7px rgba(0,0,0,0.12)",
                            "0px 11px 14px -7px rgba(0,0,0,0.2),0px 23px 36px 3px rgba(0,0,0,0.14),0px 9px 44px 8px rgba(0,0,0,0.12)",
                            "0px 11px 15px -7px rgba(0,0,0,0.2),0px 24px 38px 3px rgba(0,0,0,0.14),0px 9px 46px 8px rgba(0,0,0,0.12)",
                            "0 5px 5px -3px rgba(0,0,0,.06), 0 8px 10px 1px rgba(0,0,0,.042), 0 3px 14px 2px rgba(0,0,0,.036)" }
                },
 
                            
                /*PaletteLight = new PaletteLight
            {
                // Light mode colors
                Primary = new MudColor("#4f46e5"),       // Indigo (~oklch(0.606 0.25 292.717))
                PrimaryDarken = "#4338ca", // Slightly darker indigo
                PrimaryLighten = "#6366f1", // Slightly lighter indigo
                PrimaryContrastText = new MudColor("#ffffff"),

                Secondary = new MudColor("#f7f7f8"),     // Light gray from your --secondary
                SecondaryContrastText = new MudColor("#27272a"), // Dark text for secondary

                Tertiary = new MudColor("#f3f4f6"),      // Slightly different light gray for accent
                TertiaryContrastText = new MudColor("#27272a"),

                Background = new MudColor("#ffffff"),    // White background
                Surface = new MudColor("#ffffff"),       // White surface
                DrawerBackground = new MudColor("#fcfcfc"), // Slightly off-white drawer
                DrawerText = new MudColor("#27272a"),
                
                AppbarBackground = new MudColor("#ffffff"),
                AppbarText = new MudColor("#27272a"),
                
                TextPrimary = new MudColor("#27272a"),   // Dark text (~oklch(0.141 0.005 285.823))
                TextSecondary = new MudColor("#71717a"), // Medium gray text
                
                Dark = new MudColor("#27272a"),
                DarkContrastText = new MudColor("#ffffff"),
                
                Error = new MudColor("#ef4444"),         // Red (~oklch(0.577 0.245 27.325))
                Success = new MudColor("#10b981"),       // Green
                Warning = new MudColor("#f59e0b"),       // Amber
                Info = new MudColor("#0ea5e9"),          // Blue
                
                LinesDefault = new MudColor("#e4e4e7"),  // Border color
                TableLines = new MudColor("#e4e4e7"),
                Divider = new MudColor("#e4e4e7"),       // Divider color
                //OutlinedBorder = new MudColor("#e4e4e7"),
                
                ActionDefault = new MudColor("#71717a"),
                ActionDisabled = new MudColor("#a1a1aa"),
                ActionDisabledBackground = new MudColor("#f4f4f5"),
                
                DrawerIcon = new MudColor("#71717a"),
            },
            PaletteDark = new PaletteDark
            {
                // Dark mode colors
                Primary = new MudColor("#6366f1"),       // Indigo (~oklch(0.541 0.281 293.009))
                PrimaryDarken = "#4f46e5", 
                PrimaryLighten = "#818cf8",
                PrimaryContrastText = new MudColor("#ffffff"),
                
                Secondary = new MudColor("#3f3f46"),     // Dark gray from your --secondary-dark
                SecondaryContrastText = new MudColor("#ffffff"),
                
                Tertiary = new MudColor("#27272a"),      // Darker gray for accent
                TertiaryContrastText = new MudColor("#ffffff"),
                
                Background = new MudColor("#18181b"),    // Dark background (~oklch(0.141 0.005 285.823))
                Surface = new MudColor("#27272a"),       // Surface (~oklch(0.21 0.006 285.885))
                DrawerBackground = new MudColor("#27272a"),
                DrawerText = new MudColor("#ffffff"),
                
                AppbarBackground = new MudColor("#27272a"),
                AppbarText = new MudColor("#ffffff"),
                
                TextPrimary = new MudColor("#fafafa"),   // Light text
                TextSecondary = new MudColor("#a1a1aa"), // Medium gray text
                
                Dark = new MudColor("#18181b"),
                DarkContrastText = new MudColor("#ffffff"),
                
                Error = new MudColor("#f87171"),         // Red (~oklch(0.704 0.191 22.216))
                Success = new MudColor("#34d399"),       // Green
                Warning = new MudColor("#fbbf24"),       // Amber
                Info = new MudColor("#38bdf8"),          // Blue
                
                LinesDefault = new MudColor("#ffffff1a"), // Border color with 10% opacity
                TableLines = new MudColor("#ffffff1a"),
                Divider = new MudColor("#ffffff1a"),      // Divider with 10% opacity
                //OutlinedBorder = new MudColor("#ffffff1a"),
                
                ActionDefault = new MudColor("#a1a1aa"),
                ActionDisabled = new MudColor("#52525b"),
                ActionDisabledBackground = new MudColor("#3f3f46"),
                
                DrawerIcon = new MudColor("#a1a1aa"),
            },
            Typography = new Typography
            {
                Default = new DefaultTypography()
                {
                    FontFamily = new[] { "Inter", "ui-sans-serif", "system-ui", "sans-serif" },
                    FontSize = "0.875rem",
                    FontWeight = "400",
                    LineHeight = "1.5",
                    LetterSpacing = "normal"
                },
                H1 = new H1Typography()
                {
                    FontFamily = new[] { "Inter", "ui-sans-serif", "system-ui", "sans-serif" },
                    FontSize = "2.25rem",
                    FontWeight = "700",
                    LineHeight = "1.2",
                    LetterSpacing = "-0.025em"
                },
                H2 = new H2Typography()
                {
                    FontFamily = new[] { "Inter", "ui-sans-serif", "system-ui", "sans-serif" },
                    FontSize = "1.875rem",
                    FontWeight = "700",
                    LineHeight = "1.2",
                    LetterSpacing = "-0.025em"
                },
                H3 = new H3Typography()
                {
                    FontFamily = new[] { "Inter", "ui-sans-serif", "system-ui", "sans-serif" },
                    FontSize = "1.5rem",
                    FontWeight = "600",
                    LineHeight = "1.2",
                    LetterSpacing = "-0.025em"
                },
                H4 = new H4Typography()
                {
                    FontFamily = new[] { "Inter", "ui-sans-serif", "system-ui", "sans-serif" },
                    FontSize = "1.25rem",
                    FontWeight = "600",
                    LineHeight = "1.2"
                },
                H5 = new H5Typography()
                {
                    FontFamily = new[] { "Inter", "ui-sans-serif", "system-ui", "sans-serif" },
                    FontSize = "1.125rem",
                    FontWeight = "600",
                    LineHeight = "1.2"
                },
                H6 = new H6Typography()
                {
                    FontFamily = new[] { "Inter", "ui-sans-serif", "system-ui", "sans-serif" },
                    FontSize = "1rem",
                    FontWeight = "600",
                    LineHeight = "1.2"
                },
                Body1 = new Body1Typography()
                {
                    FontFamily = new[] { "Inter", "ui-sans-serif", "system-ui", "sans-serif" },
                    FontSize = "0.875rem",
                    FontWeight = "400",
                    LineHeight = "1.5"
                },
                Button = new ButtonTypography()
                {
                    FontFamily = new[] { "Inter", "ui-sans-serif", "system-ui", "sans-serif" },
                    FontSize = "0.875rem",
                    FontWeight = "500",
                    LineHeight = "1.75",
                    TextTransform = "none"
                }
            },
            LayoutProperties = new LayoutProperties
            {
                DefaultBorderRadius = "0.5rem",          // Matches your --radius
                DrawerWidthLeft = "260px",
                DrawerWidthRight = "260px"
            },
            Shadows = new Shadow
{
    Elevation = new string[]
    {
        "none", // 0
        "0 1px 2px rgba(0,0,0,0.04), 0 1px 1px rgba(0,0,0,0.03)", // 1 - Very subtle
        "0 2px 4px -1px rgba(0,0,0,0.04), 0 1px 2px rgba(0,0,0,0.03)", // 2
        "0 3px 5px -1px rgba(0,0,0,0.04), 0 2px 3px rgba(0,0,0,0.03)", // 3
        "0 4px 6px -2px rgba(0,0,0,0.04), 0 2px 4px rgba(0,0,0,0.03)", // 4
        "0 5px 8px -3px rgba(0,0,0,0.05), 0 3px 6px rgba(0,0,0,0.04)", // 5
        "0 6px 10px -3px rgba(0,0,0,0.05), 0 3px 8px rgba(0,0,0,0.04)", // 6
        "0 7px 12px -4px rgba(0,0,0,0.05), 0 4px 10px rgba(0,0,0,0.04)", // 7
        "0 8px 14px -4px rgba(0,0,0,0.06), 0 5px 12px rgba(0,0,0,0.05)", // 8
        "0 9px 16px -5px rgba(0,0,0,0.06), 0 5px 14px rgba(0,0,0,0.05)", // 9
        "0 10px 18px -5px rgba(0,0,0,0.07), 0 6px 16px rgba(0,0,0,0.06)", // 10
        "0 11px 20px -6px rgba(0,0,0,0.07), 0 7px 18px rgba(0,0,0,0.06)", // 11
        "0 12px 22px -6px rgba(0,0,0,0.08), 0 8px 20px rgba(0,0,0,0.07)", // 12
        "0 13px 24px -7px rgba(0,0,0,0.08), 0 9px 22px rgba(0,0,0,0.07)", // 13
        "0 14px 26px -7px rgba(0,0,0,0.09), 0 10px 24px rgba(0,0,0,0.08)", // 14
        "0 15px 28px -8px rgba(0,0,0,0.09), 0 11px 26px rgba(0,0,0,0.08)", // 15
        "0 16px 30px -8px rgba(0,0,0,0.1), 0 12px 28px rgba(0,0,0,0.09)", // 16
        "0 17px 32px -9px rgba(0,0,0,0.1), 0 13px 30px rgba(0,0,0,0.09)", // 17
        "0 18px 34px -9px rgba(0,0,0,0.11), 0 14px 32px rgba(0,0,0,0.1)", // 18
        "0 19px 36px -10px rgba(0,0,0,0.11), 0 15px 34px rgba(0,0,0,0.1)", // 19
        "0 20px 38px -10px rgba(0,0,0,0.12), 0 16px 36px rgba(0,0,0,0.11)", // 20
        "0 21px 40px -11px rgba(0,0,0,0.12), 0 17px 38px rgba(0,0,0,0.11)", // 21
        "0 22px 42px -11px rgba(0,0,0,0.13), 0 18px 40px rgba(0,0,0,0.12)", // 22
        "0 23px 44px -12px rgba(0,0,0,0.13), 0 19px 42px rgba(0,0,0,0.12)", // 23
        "0 24px 46px -12px rgba(0,0,0,0.14), 0 20px 44px rgba(0,0,0,0.13)", // 24
        "0 25px 48px -13px rgba(0,0,0,0.14), 0 20px 46px rgba(0,0,0,0.13)"  // 25
    }
}*/
                
            };
        }
    }
}

