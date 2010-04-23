# rpm spec file for Gaigen 2.5

# Basic macros
%define name    g25
%define version FILLINVERSION
%define release 1

Summary:        Geometric Algebra Code Generator 2.5
Name:           %{name}
Version:        %{version}
Release:        %{release}
License:        GPL
Group:          Development/Libraries
#BuildArch:      noarch #doesn't work
Vendor:         University of Amsterdam
URL:            http://www.geometricalgebra.org
Provides:		 g25

Source:         %{name}-%{version}.tar.gz

Buildroot: 		 %{_tmppath}/%{name}-%{version}-root 

Buildrequires:  mono-devel >= 2.4

# If the rpm finds the libraries (check the last part of the output) itself you
# shouldn't add buildrequires.
Requires:       mono-core antlr3

%description
# Gaigen 2.5 (g25) is a geometric algebra code generator.
# It compiles an XML file which describes the algebra into
# source code. Currently supported target languages are C,
# C++, CS and Java.   


%prep
%setup -q


%build
#write g25 shell script
echo "#!/bin/sh" > g25.sh
echo "" >> g25.sh
echo "mono %{_datadir}/g25/bin/g25.exe \"\$@\"" >> g25.sh
chmod 755 g25.sh
#write g25_diff shell script
echo "#!/bin/sh" > g25_diff.sh
echo "" >> g25_diff.sh
echo "mono %{_datadir}/g25/bin/g25_diff.exe \"\$@\"" >> g25_diff.sh
chmod 755 g25_diff.sh
#write g25_test_generator shell script
echo "#!/bin/sh" > g25_test_generator.sh
echo "" >> g25_test_generator.sh
echo "mono %{_datadir}/g25/bin/g25_test_generator.exe \"\$@\"" >> g25_test_generator.sh
chmod 755 g25_test_generator.sh
#write compile all
export MONO_IOMAP=all    # makes mono tools case and slash insensitive
cd g25/vs2008
xbuild g25.sln /p:Configuration=Release
cd ../../g25_diff/vs2008
xbuild g25_diff.csproj /p:Configuration=Release
cd ../../g25_test_generator/vs2008
xbuild g25_test_generator.csproj /p:Configuration=Release


%install
%__rm -rf %buildroot
install -D -m644 g25/vs2008/bin/Release/g25.exe $RPM_BUILD_ROOT%{_datadir}/g25/bin/g25.exe
install -m644 g25/vs2008/bin/Release/*.dll $RPM_BUILD_ROOT%{_datadir}/g25/bin
install -m644 g25_diff/vs2008/bin/Release/g25_diff.exe $RPM_BUILD_ROOT%{_datadir}/g25/bin/g25_diff.exe
install -m644 g25_test_generator/vs2008/bin/Release/g25_test_generator.exe $RPM_BUILD_ROOT%{_datadir}/g25/bin/g25_test_generator.exe
install -D -m755 g25.sh $RPM_BUILD_ROOT%{_bindir}/g25
install -m755 g25_diff.sh $RPM_BUILD_ROOT%{_bindir}/g25_diff
install -m755 g25_test_generator.sh $RPM_BUILD_ROOT%{_bindir}/g25_test_generator


%clean
%__rm -rf %buildroot

%files
%defattr(0644,root,root)  
%{_datadir}/g25/bin/*
%attr(0755, root, root) %{_bindir}/g25
%attr(0755, root, root) %{_bindir}/g25_diff
%attr(0755, root, root) %{_bindir}/g25_test_generator

%changelog
* Fri Apr 23 2010 Daniel Fontijne <fontijne@science.uva.nl> 1
- First version, based on rpm.skel
