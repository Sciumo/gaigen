// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

// Copyright 2008-2010, Daniel Fontijne, University of Amsterdam -- fontijne@science.uva.nl

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace G25
{
    /// <summary>
    /// Provides verbatim licenses.
    /// </summary>
    public class Licenses
    {

        public const string GPL_LICENSE =
                    "This program is free software; you can redistribute it and/or\n" +
                    "modify it under the terms of the GNU General Public License\n" +
                    "as published by the Free Software Foundation; either version 2\n" +
                    "of the License, or (at your option) any later version.\n" +
                    "\n" +
                    "This program is distributed in the hope that it will be useful,\n" +
                    "but WITHOUT ANY WARRANTY; without even the implied warranty of\n" +
                    "MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the\n" +
                    "GNU General Public License for more details.\n" +
                    "\n" +
                    "You should have received a copy of the GNU General Public License\n" +
                    "along with this program; if not, write to the Free Software\n" +
                    "Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.\n";

        public const string BSD_LICENSE =
                    "* Redistribution and use in source and binary forms, with or without\n" +
                    "* modification, are permitted provided that the following conditions are met:\n" +
                    "*     * Redistributions of source code must retain the above copyright\n" +
                    "*       notice, this list of conditions and the following disclaimer.\n" +
                    "*     * Redistributions in binary form must reproduce the above copyright\n" +
                    "*       notice, this list of conditions and the following disclaimer in the\n" +
                    "*       documentation and/or other materials provided with the distribution.\n" +
                    "*     * Neither the name of the <organization> nor the\n" +
                    "*       names of its contributors may be used to endorse or promote products\n" +
                    "*       derived from this software without specific prior written permission.\n" +
                    "*\n" +
                    "* THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDER ''AS IS'' AND ANY\n" +
                    "* EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED\n" +
                    "* WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE\n" +
                    "* DISCLAIMED. IN NO EVENT SHALL <copyright holder> BE LIABLE FOR ANY\n" +
                    "* DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES\n" +
                    "* (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;\n" +
                    "* LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND\n" +
                    "* ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT\n" +
                    "* (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS\n" +
                    "* SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.\n";

    } // end of class Licenses

} // end of namespace G25

