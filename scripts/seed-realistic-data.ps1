$ErrorActionPreference = "Stop"

$api = "http://localhost:5244/api"

function Get-Count {
    param([string]$Endpoint)

    $data = Invoke-RestMethod "$api/$Endpoint"

    if ($data -is [array]) {
        return $data.Count
    }

    if ($null -ne $data.Count) {
        return [int]$data.Count
    }

    return 0
}

function Post-Json {
    param(
        [string]$Endpoint,
        [object]$Body
    )

    $json = $Body | ConvertTo-Json -Depth 10

    return Invoke-RestMethod `
        -Uri "$api/$Endpoint" `
        -Method Post `
        -ContentType "application/json" `
        -Body $json
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host " EduSpace Allocator - Realistic Dataset" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# ---------------------------------------------------------
# 1. COMMUNITIES
# ---------------------------------------------------------

$communityNames = @(
    "Kothrud", "Karve Nagar", "Warje", "Erandwane", "Deccan",
    "Shivajinagar", "Aundh", "Baner", "Balewadi", "Pashan",
    "Bavdhan", "Paud Road", "Katraj", "Dhankawadi", "Bibwewadi",
    "Sahakarnagar", "Parvati", "Swargate", "Camp", "Wanowrie",
    "Hadapsar", "Mundhwa", "Kharadi", "Viman Nagar", "Yerawada",
    "Dhanori", "Vishrantwadi", "Kondhwa", "NIBM", "Undri"
)

$currentCommunities = Get-Count "Communities"

Write-Host "Communities currently: $currentCommunities"

for ($i = $currentCommunities; $i -lt 30; $i++) {

    $populationOptions = @(18000, 22000, 28000, 32000, 38000, 45000, 52000, 60000)
    $studentOptions = @(2800, 3500, 4200, 5000, 6200, 7500, 9000, 11000, 13000)

    $population = $populationOptions[$i % $populationOptions.Count]
    $students = $studentOptions[$i % $studentOptions.Count]

    $literacyOptions = @(68, 72, 76, 79, 82, 85, 88, 91, 93)
    $literacy = $literacyOptions[$i % $literacyOptions.Count]

    if ($i % 3 -eq 0) {
        $income = "Low"
    }
    elseif ($i % 3 -eq 1) {
        $income = "Medium"
    }
    else {
        $income = "High"
    }

    $body = @{
        population = $population
        studentCount = $students
        literacyRate = $literacy
        incomeGroup = $income
    }

    Post-Json "Communities" $body | Out-Null

    Write-Host "  Added community $($i + 1)/30"
}

# ---------------------------------------------------------
# 2. SPACES
# ---------------------------------------------------------

$spaceNames = @(
    "Kothrud Community Hall",
    "Karve Nagar Learning Room",
    "Warje Activity Centre",
    "Erandwane Study Hub",
    "Deccan Skills Room",
    "Shivajinagar Learning Centre",
    "Aundh Community Space",
    "Baner Innovation Hub",
    "Balewadi Learning Studio",
    "Pashan Community Room",
    "Bavdhan Study Centre",
    "Paud Road Learning Hub",
    "Katraj Education Room",
    "Dhankawadi Skills Centre",
    "Bibwewadi Learning Hall",
    "Sahakarnagar Community Hub",
    "Parvati Study Centre",
    "Swargate Learning Room",
    "Camp Education Hub",
    "Wanowrie Community Centre",
    "Hadapsar Learning Studio",
    "Mundhwa Study Hub",
    "Kharadi Innovation Centre",
    "Viman Nagar Learning Space",
    "Yerawada Community Room",
    "Dhanori Education Hub",
    "Vishrantwadi Study Centre",
    "Kondhwa Learning Hall",
    "NIBM Community Space",
    "Undri Learning Centre",
    "Karve Road Study Room",
    "Prabhat Road Education Hub",
    "Model Colony Learning Space",
    "Shastri Nagar Community Hall",
    "Akurdi Skills Room",
    "Pimple Saudagar Learning Hub",
    "Pimple Nilakh Study Centre",
    "Wakad Community Space",
    "Hinjawadi Learning Studio",
    "Ravet Education Hub",
    "Tathawade Study Centre",
    "Pimpri Community Room",
    "Bhosari Skills Centre",
    "Akurdi Learning Hall",
    "Nigdi Education Space",
    "Dapodi Community Hub",
    "Bopodi Learning Room",
    "Kasarwadi Study Centre",
    "Aundh Skills Lab",
    "Baner Community Classroom"
)

$coordinates = @(
    @(18.5074,73.8077), @(18.4958,73.8214), @(18.4898,73.7992),
    @(18.5115,73.8310), @(18.5165,73.8415), @(18.5308,73.8511),
    @(18.5602,73.8077), @(18.5590,73.7868), @(18.5772,73.7790),
    @(18.5432,73.7950), @(18.5340,73.7560), @(18.5105,73.7890),
    @(18.4510,73.8560), @(18.4660,73.8500), @(18.4740,73.8610),
    @(18.4880,73.8470), @(18.4900,73.8580), @(18.5010,73.8630),
    @(18.5180,73.8790), @(18.5050,73.9000), @(18.5080,73.9260),
    @(18.5300,73.9350), @(18.5510,73.9470), @(18.5670,73.9140),
    @(18.5360,73.8780), @(18.5800,73.9150), @(18.5740,73.8860),
    @(18.4750,73.8900), @(18.4630,73.9000), @(18.4500,73.9050),
    @(18.5200,73.8350), @(18.5300,73.8290), @(18.5360,73.8270),
    @(18.5450,73.8500), @(18.6480,73.7750), @(18.5980,73.7910),
    @(18.5920,73.7820), @(18.6040,73.7680), @(18.5910,73.7380),
    @(18.6240,73.7510), @(18.6270,73.7600), @(18.6260,73.8000),
    @(18.6270,73.8420), @(18.6500,73.7830), @(18.6450,73.7600),
    @(18.5900,73.8310), @(18.5700,73.8170), @(18.5750,73.8300)
)

$currentSpaces = Get-Count "Spaces"

Write-Host ""
Write-Host "Spaces currently: $currentSpaces"

for ($i = $currentSpaces; $i -lt 50; $i++) {

    $coord = $coordinates[$i % $coordinates.Count]

    $capacities = @(25, 30, 35, 40, 45, 50, 60, 70, 80, 100, 120)
    $areas = @(600, 750, 850, 1000, 1200, 1400, 1600, 1800, 2200)

    $capacity = $capacities[$i % $capacities.Count]
    $area = $areas[$i % $areas.Count]

    $rent = 7000 + (($i * 2300) % 23000)

    $available = $true

    if ($i % 9 -eq 0) {
        $available = $false
    }

    $buildingName = $spaceNames[$i]

    if ([string]::IsNullOrWhiteSpace($buildingName)) {
        $buildingName = "Community Learning Space $($i + 1)"
    }

    $body = @{
        buildingName = $buildingName
        address = "$buildingName, Pune, Maharashtra"
        city = "Pune"
        latitude = [double]$coord[0]
        longitude = [double]$coord[1]
        floorArea = [double]$area
        capacity = [int]$capacity
        rentalCost = [decimal]$rent
        availability = $available
    }

    Post-Json "Spaces" $body | Out-Null

    Write-Host "  Added space $($i + 1)/50"
}

# ---------------------------------------------------------
# 3. LEARNING REQUESTS
# ---------------------------------------------------------

$organizations = @(
    "Pune Learning Foundation",
    "CodeForCommunity",
    "Digital Maharashtra NGO",
    "STEM Pune Initiative",
    "Youth Skills Network",
    "Community Education Trust",
    "Future Coders Pune",
    "Rural Digital Learning",
    "Women Tech Learning",
    "Pune Science Club",
    "Open Skills Foundation",
    "Urban Learning Mission",
    "TechReach NGO",
    "Bright Futures Trust",
    "Smart Education Collective"
)

$programs = @(
    "Coding",
    "Mathematics",
    "Robotics",
    "Digital Literacy",
    "STEM",
    "English",
    "Data Science",
    "Web Development",
    "Computer Basics",
    "AI Fundamentals"
)

$requestCoordinates = @(
    @(18.508,73.810), @(18.496,73.823), @(18.491,73.801),
    @(18.513,73.833), @(18.518,73.843), @(18.532,73.853),
    @(18.562,73.809), @(18.561,73.789), @(18.579,73.781),
    @(18.545,73.797), @(18.536,73.758), @(18.512,73.791),
    @(18.453,73.858), @(18.468,73.852), @(18.476,73.863),
    @(18.490,73.849), @(18.492,73.860), @(18.503,73.865),
    @(18.520,73.881), @(18.507,73.902), @(18.510,73.928),
    @(18.532,73.937), @(18.553,73.949), @(18.569,73.916),
    @(18.538,73.880), @(18.582,73.917), @(18.576,73.888),
    @(18.477,73.892), @(18.465,73.902), @(18.452,73.907)
)

$currentRequests = Get-Count "LearningRequests"

Write-Host ""
Write-Host "Learning requests currently: $currentRequests"

for ($i = $currentRequests; $i -lt 30; $i++) {

    $coord = $requestCoordinates[$i % $requestCoordinates.Count]

    $studentCapacities = @(20, 25, 30, 35, 40, 45, 50, 60, 70, 80, 90, 100)
    $studentCapacity = $studentCapacities[$i % $studentCapacities.Count]

    # Some requests intentionally have budgets below common rents,
    # producing infeasible matching candidates.
    if ($i % 7 -eq 0) {
        $budget = 6000
    }
    elseif ($i % 5 -eq 0) {
        $budget = 9000
    }
    else {
        $budget = 10000 + (($i * 1800) % 30000)
    }

    $communityId = ($i % 30) + 1

    $body = @{
        organization = $organizations[$i % $organizations.Count]
        programType = $programs[$i % $programs.Count]
        studentCapacity = [int]$studentCapacity
        budget = [decimal]$budget
        preferredLocation = "Pune"
        preferredLatitude = [double]$coord[0]
        preferredLongitude = [double]$coord[1]
        communityId = [int]$communityId
    }

    Post-Json "LearningRequests" $body | Out-Null

    Write-Host "  Added learning request $($i + 1)/30"
}

# ---------------------------------------------------------
# 4. PARTNERS
# ---------------------------------------------------------

$partnerOrganizations = @(
    "Pune Municipal Education Cell",
    "Tech Maharashtra Foundation",
    "Digital India Learning Trust",
    "Community Skills Pune",
    "STEM Outreach Network",
    "Urban Education Initiative",
    "Pune Youth Development Forum",
    "Learning Without Limits",
    "Open Knowledge Pune",
    "Future Skills Foundation",
    "Women in Technology Pune",
    "Smart City Education Group",
    "Community Coding Alliance",
    "Education Access Trust",
    "Pune Innovation Network"
)

$currentPartners = Get-Count "Partners"

Write-Host ""
Write-Host "Partners currently: $currentPartners"

for ($i = $currentPartners; $i -lt 15; $i++) {

    $body = @{
        organizationName = $partnerOrganizations[$i]
        partnerType = if ($i % 3 -eq 0) { "NGO" } elseif ($i % 3 -eq 1) { "Government" } else { "Education" }
        contactPerson = "Coordinator $($i + 1)"
        email = "partner$($i + 1)@eduspace.org"
        phone = "987650$("{0:D4}" -f ($i + 1))"
        status = "Active"
    }

    Post-Json "Partners" $body | Out-Null

    Write-Host "  Added partner $($i + 1)/15"
}

# ---------------------------------------------------------
# 5. COURSES
# ---------------------------------------------------------

$courses = @(
    @("Weekend Coding", "Programming", 40, 2, "Beginner"),
    @("Python Fundamentals", "Programming", 35, 3, "Beginner"),
    @("Web Development", "Technology", 45, 3, "Intermediate"),
    @("Robotics Basics", "STEM", 30, 3, "Beginner"),
    @("Advanced Robotics", "STEM", 50, 4, "Advanced"),
    @("Digital Literacy", "Digital Skills", 60, 2, "Beginner"),
    @("Computer Fundamentals", "Technology", 50, 2, "Beginner"),
    @("Data Science Basics", "Data", 35, 4, "Intermediate"),
    @("Artificial Intelligence", "AI", 40, 4, "Advanced"),
    @("Mathematics Support", "Education", 70, 2, "Beginner"),
    @("English Communication", "Language", 50, 2, "Beginner"),
    @("Spoken English", "Language", 40, 2, "Intermediate"),
    @("Scratch Programming", "Programming", 30, 2, "Beginner"),
    @("Java Fundamentals", "Programming", 35, 3, "Intermediate"),
    @("Cyber Safety", "Technology", 60, 2, "Beginner"),
    @("IoT Introduction", "Technology", 40, 3, "Intermediate"),
    @("Electronics Basics", "Engineering", 35, 3, "Beginner"),
    @("STEM Discovery", "STEM", 50, 2, "Beginner"),
    @("Career Skills", "Professional Skills", 80, 2, "Beginner"),
    @("Entrepreneurship", "Business", 45, 3, "Intermediate")
)

$currentCourses = Get-Count "Courses"

Write-Host ""
Write-Host "Courses currently: $currentCourses"

for ($i = $currentCourses; $i -lt 20; $i++) {

    $course = $courses[$i]

    $body = @{
        courseName = $course[0]
        category = $course[1]
        requiredCapacity = [int]$course[2]
        durationHours = [int]$course[3]
        skillLevel = $course[4]
    }

    Post-Json "Courses" $body | Out-Null

    Write-Host "  Added course $($i + 1)/20"
}

# ---------------------------------------------------------
# SUMMARY
# ---------------------------------------------------------

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host " DATASET SEEDING COMPLETE" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green

Write-Host ""
Write-Host "Spaces:            $(Get-Count 'Spaces')"
Write-Host "Communities:       $(Get-Count 'Communities')"
Write-Host "Learning Requests: $(Get-Count 'LearningRequests')"
Write-Host "Partners:          $(Get-Count 'Partners')"
Write-Host "Courses:           $(Get-Count 'Courses')"
Write-Host ""


