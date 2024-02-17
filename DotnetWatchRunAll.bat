SET BASE=%cd%

cd "%BASE%\Demos\Blazorise.Demo.Bootstrap"
start dotnetwatchrun

:bootstrap
SET URL="http://localhost:7713"
SET curlCommand=curl -LI %URL% -o /dev/null -w "%%{http_code}" -s 
@for /f %%R in ('%curlCommand%') do ( Set status=%%R )

if %status% == 200 (
    echo  next
) else (
    goto bootstrap
)

cd "%BASE%\Demos\Blazorise.Demo.Bootstrap5"
start dotnetwatchrun 

:bootstrap5
SET URL="http://localhost:8713"
SET curlCommand=curl -LI %URL% -o /dev/null -w "%%{http_code}" -s 
@for /f %%R in ('%curlCommand%') do ( Set status=%%R )

if %status% == 200 (
    echo  next
) else (
    goto bootstrap5
)

cd "%BASE%\Demos\Blazorise.Demo.Bulma"
start dotnetwatchrun

:bulma
SET URL="http://localhost:12840"
SET curlCommand=curl -LI %URL% -o /dev/null -w "%%{http_code}" -s 
@for /f %%R in ('%curlCommand%') do ( Set status=%%R )

if %status% == 200 (
    echo  next
) else (
    goto bulma
)

cd "%BASE%\Demos\Blazorise.Demo.Material"
start dotnetwatchrun

:material
SET URL="http://localhost:14302"
SET curlCommand=curl -LI %URL% -o /dev/null -w "%%{http_code}" -s 
@for /f %%R in ('%curlCommand%') do ( Set status=%%R )

if %status% == 200 (
    echo  next
) else (
    goto material
)

cd "%BASE%\Demos\Blazorise.Demo.Tailwind"
start dotnetwatchrun

:tailwind
SET URL="http://localhost:5220"
SET curlCommand=curl -LI %URL% -o /dev/null -w "%%{http_code}" -s 
@for /f %%R in ('%curlCommand%') do ( Set status=%%R )

if %status% == 200 (
    echo  next
) else (
    goto tailwind
)

cd "%BASE%\Demos\Blazorise.Demo.AntDesign"
start dotnetwatchrun

:antdesign
SET URL="http://localhost:17715"
SET curlCommand=curl -LI %URL% -o /dev/null -w "%%{http_code}" -s 
@for /f %%R in ('%curlCommand%') do ( Set status=%%R )

if %status% == 200 (
    echo  next
) else (
    goto antdesign
)

cd "%BASE%\Demos\Blazorise.Demo.FluentUI2"
start dotnetwatchrun

exit