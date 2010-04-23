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

/*! \mainpage Gaigen 2.5 library (libg25) Documentation
 *
 * 
 * Released under GPL license.
 * 
 * \section intro_sec Introduction
 *
 * Gaigen 2.5 is a geometric algebra code generator. It turns a specification of geometric algebra into
 * an implementation of that algebra in a specific language.
 * 
 * This library handles all the internals: reading the specification, generating the code, etc.
 * The other programs such as G25 are just shells around this library. The actual code generation
 * functionality is handled by plugins which implement the interface G25.CodeGenerator.
 * 
 * \section classes_sec The classes
 * 
 * The G25.Specification class carries all information on the specification of an algebra. It can
 * read and write XML files containing the specification (see \ref specification_sec for a description).
 * 
 * The G25.rsep class is a simple expression parser that Gaigen uses to parse parts of the XML
 * specification, such as the metric specification (<c>"e1.e1=1"</c>)and the definition of basis vectors (<c>"e1^e2^e3"</c>).
 * The G25.rsel class is a simple expression lexer used by the parser.
 * 
 * The G25.rsbbp class is a parser for list of basis blades as they are stored in the specification file.
 * It uses G25.rsep to parse simple strings like <c>"no=1 e1 e2 e3"</c> or <c>"e1^e2 e2^e3 e3^e1"</c>.
 * 
 * The G25.MV class (and its subclasses G25.SMV and G25.GMV) represent multivector classes
 * which will be generated. The main properties of these classes are their names and the order
 * and possibly grouping of coordinates.
 * 
 * The G25.OM class (and its subclasses G25.SOM and G25.GOM) represent outermorphism matrix
 * representations which will be generated. The main properties of these classes are their names 
 * and the order and coordinates in the domain and the range.
 * 
 * The G25.fgs class holds Function Generation Specifications. This means a request of the
 * code generator to generate a specific function over specific multivector/outermorphism arguments,
 * with specific floating point types.
 * 
 * The G25.CodeGeneratorLoader handles the loading of G25.CodeGenerator and G25.CodeGeneratorPlugin  
 * classes.These code generators generate the code for a specific language, given the G25.Specification.
 * 
 * The G25.UserException should be thrown when an error occurs which is due to the user (i.e., and error in the input XML file).
 * These are caught and presented to the user as errors.
 * 
 * \section specification_sec The specification XML
 * 
 * This section describes the XML specification file format. An example of a full specification is
 * given in section \ref specification_example_sec "Example of specification XML". 
 * First a description of each of the elements is given.
 *  
 * The XML specification start with the opening element <c>g25spec</c>.
 * This element can have the following attributes, most of which are required.
 *   - <c>license</c>. The license of the generated code. The value can be <c>custom</c>, <c>gpl</c> or <c>bsd</c>. If the license is custom, a <c>customLicense</c> is expected.
 *   - <c>language</c>. The value can be <c>c</c>, <c>cpp</c>, <c>java</c>, <c>csharp</c>, <c>python</c>, <c>matlab</c> currently. The license is case insensitive.
 *     (the fact that a value is valid does not means that it is actually implemented . . .). The language names are case insensitive.
 *   - <c>namespace</c>. The name and the namespace of the generated code (always required, because it is also used as a prefix/part of generated filenames).
 *   - <c>coordStorage</c>. The value can be <c>array</c> (coordinates are stored in arrays) or <c>variables</c> (one variable for each coordinate). This only applies to specialized multivectors.
 *   - <c>defaultOperatorBindings</c>. The value can be <c>true</c> or <c>false</c>. If true, the default operator bindings for the output language are used (e.g., <c>+</c> -> <c>add</c>).
 *   - <c>dimension</c>. The dimension of the space of the algebra. Must be >= 1 and if the dimension is larger than about 8, code generation will become pretty slow.
 *   - <c>reportUsage</c>. The value can be <c>true</c> or <c>false</c>. If true, print statements are added to the code to eport usage of non-optimized functions
 *      (i.e., functions involving specialized multivectors which were implicitly converted to general multivectors). When true, a field is added to the GMV type which keeps track of the original specialized 
 *      type of the multivector. This option has no effect in the <c>C</c> language because it does not support implicit conversion.
 *   - <c>gmvCode</c>. Possible values are <c>expand</c> and <c>runtime</c>. The code for general multivectors can be very large. For example a geometric product of two GMVs in 10-D 
 *     takes in the order of 1024*1024 multiplication and additions. If the code to compute this is write explicitly into the code (the default option),
 *     the codesize would also be in the order of megabytes. Because in practice if this Gaigen 1 and Gaigen 2 could only generate algebras up to 7-D.
 *     To overcome this limitation, Gaigen 2.5 supports 'run-time' computation of geometric products and all other functions without explicitly generating
 *     code for every mul/add. The default option <c>expand</c> writes out all code and is not really useable above 7-D. The option <c>runtime</c> 
 *     performs computations at run-time, using (among others) tables which must be initialized at startup. If the option <c>runtime</c> is used,
 *     the metric must be diagonal. Otherwise Gaigen 2.5 cannot compute these tables at runtime
 *     This is because non-diagonal metrics requires symmetric eigenvalue computations, and 
 *     it would be a burden to require eigenvalue code for every output language. Note that the run-time code is slower than the expanded code.
 *   - <c>parser</c>. What type of multivector string parser to generate. The default is <c>none</c>. Other options are
 *      <c>builtin</c> (for a parser hand-written for Gaigen 2.5) and <c>antlr</c> for an ANTLR based parser. Both these parsers have the
 *      same functionality and interface, but their implementation is different of course. For the ANTLR parser, you need to
 *      invoke <c>java org.antlr.Tool</c> on the generated <c>.g</c> grammar and link with the ANTLR run-time.
 *   - <c>copyright</c>. The copyright statement of the generated code.
 * 
 * Inside the <c>g25spec</c> element, the following elements can be present:
 *   - <c>customLicense</c>. The custom license text. Must be present when <c>license="custom"</c>. Copied verbatim to the top of each the generated file.
 *   - <c>outputDirectory</c>. Where the generated files should go. The value <c>path</c> attribute is the name of the path.
 *      By default, output is to the current working directory.
 *   - <c>outputFilename</c>. Allows the name of individual generated files to be modified. For example, if the code generator 
 *      would generate a file named 'foo.cpp', but the user wants this file to be named 'bar.cpp', then setting 
 *      attributes <c>defaultName="foo.cpp"</c> and <c>customName="bar.cpp"</c>
 *      allows the filename to be overridden. Attributes:
 *        - <c>defaultName</c> (required). Name that the file would otherwise have (do not include path).
 *        - <c>customName</c> (required). Custom name for file (do not include path).
 *   - <c>inline</c>. What type of functions to inline. Possible attributes are <c>constructors</c>, 
 *     <c>set</c>, <c>assign</c>, <c>operators</c> and <c>functions</c>.
 *     The value of the attributes can be <c>true</c> or <c>false</c>.
 *   - <c>floatType</c>. This element can have the following attributes:
 *        - <c>type</c> (required). The value should be a floating point type (e.g. <c>float</c> or <c>double</c>).
 *        - <c>suffix</c> (optional). The suffix applied to multivector/outermorphism classes when instantiated with this
 *           floating point type. For example if there is a specialized multivector called <c>vectorE3GA</c> and the suffix
 *           for the float type <c>double</c> is <c>_f</c> then <c>vectorE3GA</c> instantiated with <c>double</c>
 *           will be called <c>vectorE3GA_t</c>.
 *        - <c>prefix</c> (optional). The prefix applied to multivector/outermorphism classes when instantiated with this
 *           floating point type.
 *   - <c>unaryOperator</c>. This element allows you to tie an unary operator symbol to a one-argument function (in
 *     languages which support this). The attributes of this element are:
 *        - <c>symbol</c>. The operator symbol, for example <c>++</c>.
 *        - <c>prefix</c>. Only for <c>++</c> and <c>--</c>:
 *           Whether this operator is prefix (e.g. <c>++a</c>) or postifx (<c>a++</c>). Use <c>true</c>
 *           for prefix, and <c>false</c> for postfix. 
 *        - <c>function</c>. The name of the function to bind to, for example <c>preIncrement</c>.
 *   - <c>binaryOperator</c>. This element allows you to tie an binary operator symbol to a two-argument function (in
 *     languages which support this). The attributes of this element are:
 *        - <c>symbol</c>. The operator symbol, for example <c>^</c>.
 *        - <c>function</c>. The name of the function to bind to, for example <c>op</c>.
 *   - <c>basisVectorNames</c>. This element lists the names of basis vectors of the algebra. The number of basis vectors
 *     must match the dimension <c>N</c> of the space. The attributes of the element are <c>name1</c>, <c>name2</c>, 
 *     ..., <c>nameN</c>. Each attribute is assigned the name of its respective basis vector, for example <c>name1="e1"</c>.
 *   - <c>metric</c>. A <c>metric</c> element specifies the inner product between one or more pairs of basis vectors.
 *     By default, all inner product between basis vectors are assumed to be 0. By using <c>metric</c> elements, one can
 *     set the inner product to different values. Inside one algebra, different metrics can be used, e.g. a conformal one and
 *     a Euclidean one. This is used internally by Gaigen for generating the code for e.g. factorization, but may also be
 *     useful for end-users.
 *     An example of a metric element is 
 *     <code>
 *     <metric name="default">no.ni=-1</metric>
 *     </code>
 *     This line says that the inner product between basis vectors <c>no</c> and <c>ni</c> is <c>-1</c>.
 *     The attribute <c>name="default"</c> says that this line belongs to the "default" metric and may be
 *     left out (because the default value for this attribute is <c>"default"</c>.
 *     One may also specify multiple metrics at once, as in
 *     <code>
 *     <metric>e1.e1=e2.e2=e3.e3=1</metric>
 *     </code>
 *     Inside <c>function</c> elements, a non-default metric may be specified by using the 
 *     <c>metric="name"</c> attribute.
 *     Due to floating point roundoff errors in eigenvalue computation, a value or
 *     coordinates that should be (e.g.)1.0 may become (e.g.)(1.0+-1e-16).
 *     This makes the generated code less efficient, is annoiying to read and propagates
 *     the roundoff errors.
 *     For that reason, there is the option to round coordinates after metric product.
 *     The default is to round, but when the final metric is diagonal, it is forced
 *     to no rounding because there is not need to use it in that case. 
 *     The user can explicitly specify the rounding using the <c>round="false"</c> or <c>round="true"</c> attribute,
 *     but when the metric is diagonal, it will still be forced to no rounding.
 *     When rounding is enabled, coordinates which are very close to an integer
 *     value are 'rounded' to that value. The threshold for being 'very close' is 1e-14.
 *   - <c>mv</c>. This element specifies the properties of the general multivector. It is one of the most involved elements.
 *     Its attributes are:
 *        - <c>name</c>. The name of the general multivector type, for example <c>mv</c>.
 *        - <c>compress</c>. How to compress the multivector coordinates: <c>byGrade</c> or <c>byGroup</c> (explained in detail \ref mv_coord_sec "below").
 *        - <c>coordinateOrder</c>. The order of coordinates: <c>default</c> or <c>custom</c> (explained in detail \ref mv_coord_sec "below").
 *        - <c>memAlloc</c>. How to allocate memory for coordinates: <c>full</c>, <c>parityPure</c> or <c>dynamic</c> (explained in detail \ref mv_coord_sec "below").
 *   - <c>smv</c>. An <c>smv</c> specifies a specialized multivector type. 
 *     The <c>smv</c> element should contain the basis blades of the type. These may have constant assignments, and if the
 *     type is constant <c>const="true"</c>, then all basis blades must have a constant assignment. An example of a specialized
 *     multivector definition is:
 *     <code>
 *     <smv name="normalizedPoint" type="blade">no=1 e1 e2 e3</smv>
 *     </code>
 *     The attributes of a <c>smv</c> element are:
 *        - <c>name</c>. The name of the specialized multivector type, for example <c>vector</c>.
 *        - <c>const</c> (optional). Can either be <c>true</c> or <c>false</c>. When true, the type is to be a constant type with no
 *          variable coordinates. In that case, all basis blades must have a constant value assigned to it. If the <c>const</c>
 *          attribute is not specified it is assumed to be <c>false</c>.
 *        - <c>type</c>. The <c>type</c> attribute specified whether instances of the specifialized multivector class will carry
 *          blade (<c>type="blade"</c>), rotor (<c>type="rotor"</c>), versor (<c>type="versor"</c>) or any multivector 
 *          (<c>type="multivector"</c>) values. This may be used for optimizations and sanity checks by code generator back-ends.
 *   - <c>constant</c>. This element is used to generate a constant value in the output. This is useful is you
 *     want a constant value of an existing type. The constant has a name, 
 *     a type, and a value. Some examples of a constant are:
 *     <code>
 *     <constant name="vectorE1" type="vectorE3GA">e1=1 <comment>e1, as a vectorE3GA</comment></constant>
 *     <constant name="pointAtOrigin" type="normalizedPoint">e1=0 e2=0 e3=0 ni=0</constant>
 *     </code>
 *     Note that coordinates which are zero do not need to be specified. The attributes of a <c>smv</c> element are:
 *        - <c>name</c>. The name of the constant.
 *        - <c>type</c>. The type of the constant. Currently only specialized multivector constants are supported (<c>smv</c>).
 *     The <c>constant</c> contains the value of the coordinates of the constant, and optionally a <c>comment</c> element.
 *   - <c>om</c>. Specifies the general outermorphism matrix representation type. The outermorphism has a domain and
 *     a range, both of which may be specified, but they can also be set to the defaults. An example of an outermorphism with default
 *     coordinate order is:
 *     <code>
 *     <om name="om" coordinateOrder="default" />
 *     </code>
 *     A 3-D example of an outermorphism with a custom domain and range is:
 *     <code>
 *     <om name="om" coordinateOrder="custom">
 *     <domain>scalar e1 e2 e3 e1^e2 e2^e3 e3^e1 e1^e2^e3</domain>
 *     <range>scalar e1 e2 e3 e1^e2 e2^e3 e3^e1 e1^e2^e3</range>
 *     </code>
 *     In this last example, it was redundant to specify the range since it is identical to the domain. Leaving the <c>range</c> element
 *     out would have the same effect. Note that all basis blades must be present in an general outermorphism's range and domain.
 *     The attributess of a <c>om</c> element are:
 *        - <c>name</c>. The name of the outermorphism type, for example <c>om</c>.
 *        - <c>coordinateOrder</c>. This can be <c>default</c> or <c>custom</c>. If <c>custom</c> is used, the <c>domain</c>
 *           and possibly the <c>range</c> should be specified. If the <c>range</c> is left out, it is assumed to be identical to the <c>domain</c>.
 *   - <c>som</c>. A <c>som</c> element specifies a specialized outermorphism. It is pretty much that same as a general outermorphism
 *     except it does not need to have all basis blades in its domain and range. An example of a <c>som</c> element is:
 *     <code>
 *     <som name="flatPointOM">
 *     <domain>e1^ni e2^ni e3^ni no^ni</domain>
 *     <range>e1^ni e2^ni e3^ni no^ni</range> 
 *     </som>
 *     </code>
 *     The <c>som</c> element has only one attribute, since the <c>coordinateOrder</c> is always custom:
 *        - <c>name</c>. The name of the outermorphism type, for example <c>om</c>.
 *   - <c>function</c>. This element specifies a request to the code generator back-end to implement a specific function
 *     for specific arguments.
 *     The attributes are:
 *        - <c>name</c>. The name of the function, as it is known to the code generator. This name is also the name of the
 *           generated function unless an <c>outputName</c> attribute is specified. To generate a converter (underscore constructor),
 *           the name of the function should be the destination type plus an underscore, e.g., <c>_vectorE3GA</c>. This first (and only)
 *           argument should be the source type.
 *        - <c>outputName</c>. Optional. Changes the name of the generated function to the value of the attribute. For example, allows you
 *          to rename a function <c>gp</c> to <c>geometricProduct</c>
 *        - <c>returnType</c>. Optional. By default, the code generator will determine the return type of the functions it generates, but it is possible to override this default by setting it explicitly. 
 *          The return type should be the name of a specialized mutlvector. However, the return type may also be 'scalar' or any of the floating point typenames used in the algebra.
 *          If the return type is 'scalar', then a float will be returned, automatically adapted to the floating point type of the function.
 *        - <c>argN</c>. Specifies the type of argument <c>N</c>. If no <c>argN</c> attribute is given, the code generator should fill in the right number of general multivector or general 
 *           outermorphism types automatically. Otherwise, the correct number of <c>argN</c> attributes should be specified for the function (running from 1 up
 *           to the number of arguments of the function). Not all combinations of argument types are possible. For example, currently it is not possible
 *           to mix general and specialized multivectors. It is possible to mix floating point types and general multivectors though.
 *        - <c>argNameN</c>. Specifies the name of argument <c>N</c>. This only affects the name of the argument inside
 *           the generated function. Specifying this name may be superfluous, but may improve readability, especially for code-completion
 *           wizards. 
 *        - <c>optionX</c>. Specifies a optional option 'X'. For example, the <c>exp</c> functions can generate more efficient code
 *           when it knows what the sign of the square of the argument is. In that case, one may use for example <c>optionSquare="1.0"</c>.
 *        - <c>floatType</c>. Multiple <c>floatType</c> attributes may be present in a single <c>function</c> element. 
 *          By default, the code generator will generate code for all floating point types of the specification, but using the 
 *          <c>floatType</c> attribute(s) this may be limited to only the set of listed floating point types.
 *        - <c>metric</c>. The optional <c>metric</c> attribute specifies the usage of a non-default metric (case insensitive). By default,
 *          the metric <c>"default"</c> is used. By using this attribute a different metric may be used for
 *          the function, e.g., <c>metric="euclidean"</c>.
 *        - <c>comment</c>. Use the this optional attribute to add any extra comment to the function documentation. For example, one could
 *          use the comment to explain what a certain function is used for.
 *   - <c>verbatim</c>. This element is used to add verbatim code to the output files. This may be useful to
 *     include some headers, to add some custom functions or documentation. The <c>verbatim</c> element can contain
 *     the code as text, or can point to a file using the <c>codeFilename</c> attribute. The attributes are:
 *        - <c>filenameX</c>. The filename(s) of the files to modify. Multiple files can be modifed with one <c>verbatim</c> element.
 *        The <c>X</c> in the attribute can be any string (including empty). If multiple <c>filenameX</c> attributes are specified, multiple
 *        files are modified.
 *        - <c>position</c>. Where to place the verbatim code. The values can be <c>top</c> (at the top of the file), 
 *           <c>bottom</c> (at the bottom of the file), <c>before</c> (before some marker string) or
 *           <c>after</c> (before some marker string).
 *        - <c>marker</c>. If <c>position</c> is <c>before</c> or <c>after</c>, then this attribute specifies the string
 *           before or after which the verbatim code should be inserted.
 *        - <c>codeFilename</c>. The verbatim code can be directly inside the <c>verbatim</c> element but for long code
 *           it may be easier to put the code in a separate file. The name of this file is specified using this attribute.
 * \subsection mv_coord_sec Multivector compression, coordinate order and memory allocation
 * 
 * Memory of general multivector variables can be allocated in different ways whose effictiveness will vary per code generator/language.
 * You can allocate memory for all possible coordinates (<c>memAlloc="full"</c>). This would for example allocate 32 coordinates for a 5-D algebra.
 * You can also allocate half of that if you know that you will always use parity-pure (only even-grade, or only odd-grade) multivectors
 * (<c>memAlloc="parityPure"</c>). Another option is dynamically allocate just the memory that is required (<c>memAlloc="dynamic"</c>).
 * 
 * Compression of multivector coordinates can be done per grade part (<c>compress="byGrade"</c>) or per user-defined group (<c>compress="byGroup"</c>).
 * 
 * If compression is done by grade, then the attribute value <c>coordinateOrder="default"</c> can be used. But it is also allowed to
 * have a custom coordinate order. In that case the <c>mv</c> element must contain a list of basis blades all, i.e., the order of coordinates.
 * The basis blades should not be in <c>group</c> elements, just listed in the order you want them to be. The basis blades
 * should be listed in ascending grade order. For example:
 * <code>
 <mv compress="byGrade" coordinateOrder="custom" memAlloc="parityPure">
    scalar
    no e1 e2 e3 ni
    no^e1 no^e2 no^e3 e1^e2 e2^e3 e3^e1 e1^ni e2^ni e3^ni no^ni
    e2^e3^ni e3^e1^ni e1^e2^ni no^e3^ni no^e1^ni no^e2^ni no^e2^e3 no^e1^e3 no^e1^e2 e1^e2^e3
    e1^e2^e3^ni no^e2^e3^ni no^e1^e3^ni no^e1^e2^ni no^e1^e2^e3
    no^e1^e2^e3^ni
</mv>
 *  </code>
 * If compression is done by group, each group of basis blades must be specified inside the <c>mv</c> element inside a <c>group</c> element. 
 * A group cannot contain basis blades of different grades. This example splits the coordinates of the 5-D conformal algebra into
 * three basic groups (<c>no</c>, <c>ni</c>, and <c>e1, e2, e3</c>) for all grades. For example:
 *  <code>
<mv compress="byGroup" coordinateOrder="custom" memAlloc="parityPure">
    <group>scalar</group>
    <group>no</group>
    <group>e1 e2 e3</group>
    <group>ni</group>
    <group>no^e1 no^e2 no^e3</group>
    <group>e1^e2 e2^e3 e3^e1</group> 
    <group>e1^ni e2^ni e3^ni</group>
    <group>no^ni</group>
    <group>e2^e3^ni e3^e1^ni e1^e2^ni</group>
    <group>no^e3^ni no^e1^ni no^e2^ni</group>
    <group>no^e2^e3 no^e1^e3 no^e1^e2</group>
    <group>e1^e2^e3</group>
    <group>e1^e2^e3^ni</group>
    <group>no^e2^e3^ni no^e1^e3^ni no^e1^e2^ni</group>
    <group>no^e1^e2^e3</group>
    <group>no^e1^e2^e3^ni</group>
</mv>
 *  </code>
 * 
 * 
 * \section specification_example_sec Example of specification XML
 *
 * An example of a specification is (TO DO: better example specification):
 * <code>
<?xml version="1.0" encoding="utf-8" ?>

<g25spec 
	license="custom" 
	language="cpp"
	namespace="c3ga"
	coordStorage="array"
	defaultOperatorBindings="true"
	dimension="5"
	gmvCode="expand"
	parser="builtin"
	copyright="Copyright (C) 2008 Some Random Person"
	>
	
<customLicense>put custom license text here</customLicense>

<outputDirectory path="/tmp/"/>
<outputFilename defaultName="foo.cpp" customName="bar.cpp"/>

 
<inline 
	constructors="true" 
	set="true" 
	assign="true" 
	operators="true" 
	functions="true"
	/>


<floatType type="double" suffix=""/>
<floatType type="float" suffix="_f"/>

<unaryOperator symbol="-" prefix="true" function="negate"/>
<binaryOperator symbol="^" function="op"/>

<basisVectorNames 
	name1="no"
	name2="e1"
	name3="e2"
	name4="e3"
	name5="ni"
	/>
	
<metric>no.ni=-1</metric>
<metric>e1.e1=e2.e2=e3.e3=1</metric>

<mv name="mv" compress="byGrade" coordinateOrder="default" memAlloc="parityPure"/>

<smv name="normalizedPoint" type="blade">no=1 e1 e2 e3</smv>
<smv name="no" const="true" type="blade">no=1</smv>
<smv name="e1" const="true" type="blade">e1=1</smv>
<smv name="e2" const="true" type="blade">e2=1</smv>
<smv name="e3" const="true" type="blade">e3=1</smv>
<smv name="ni" const="true" type="blade">ni=1</smv>
<smv name="I3" const="true" type="blade">e1^e2^e3=1</smv>
<smv name="I5" const="true" type="blade">no^e1^e2^e3^ni=1</smv>
<smv name="I5i" const="true" type="blade">no^e1^e2^e3^ni=-1</smv>
<smv name="vectorE3GA" type="blade">e1 e2 e3</smv>
<smv name="bivectorE3GA" type="blade">e1^e2 e2^e3 e3^e1</smv>
<smv name="rotorE3GA" type="versor">scalar e1^e2 e2^e3 e3^e1</smv>

<om name="om" coordinateOrder="default" />

<som name="flatPointOM">
<domain>e1^ni e2^ni e3^ni no^ni</domain>
<range>e1^ni e2^ni e3^ni no^ni</range> 
</som>

<som name="strangeOM">
<domain>e1^ni e2^ni e3^ni no^ni</domain>
<range>no^ni</range> 
</som>

<function name="gp"/> 
<function name="op"/>
<function name="lc"/>
<function name="rc"/>
<function name="factor" outputName="factorize" />
<function name="_vectorE3GA" arg1="normalizedPoint" argName1="P" floatType="float"/>
<function name="gp" arg1="vectorE3GA" arg2="normalizedPoint" floatType="float"/>
<function name="op" arg1="vectorE3GA" arg2="normalizedPoint" /> 
<function name="exp" arg1="bivectorE3GA" returnType="rotorE3GA"/>

</g25spec>
 * </code>.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;


namespace G25
{

    /// <summary>
    /// Output language for generated code.
    /// </summary>
    public enum OUTPUT_LANGUAGE {
        NONE = -1,
        C = 0,
        CPP,
        JAVA,
        CSHARP,
        PYTHON,
        MATLAB
    }

    /// <summary>
    /// How to store coordinates of specialized multivectors: in arrays or as single variables.
    /// </summary>
    public enum COORD_STORAGE
    {
        ARRAY = 1,
        VARIABLES = 2
    }

    /// <summary>
    /// Expand general multivector code, or compute tables it at run-time
    /// </summary>
    public enum GMV_CODE
    {
        EXPAND = 1,
        RUNTIME = 2
    }

    /// <summary>
    /// What type of parser to use. <c>CUSTOM</c> means a hand-written
    /// parser.
    /// </summary>
    public enum PARSER
    {
        NONE=0,
        BUILTIN=1,
        ANTLR=2
    }

    /// <summary>
    /// This class represents a Gaigen 2.5 specification.
    /// 
    /// A specification contains all information about the algebra to be generated: license, namespace,
    /// specialized multivectors, what functions to generate, optimize, and so on.
    /// 
    /// The constructor G25.Specification.Specification(string filename) constructs a new specification
    /// from an XML file.
    /// 
    /// A specification can be converted to XML using G25.Specification.ToXmlString(). This function returns
    /// an XML string representing the specification, formatted for human readability.
    /// 
    /// All specification items are currently stored in public member variables, so in theory you could
    /// set them directly. For most members, there are appropriate function to set them, such as
    /// SetDimension(), SetBasisVectorName(), SetMetric() and AddOperator().
    /// </summary>
    public class Specification
    {
        public static String FullGaigenName = "Gaigen 2.5";

        /// <summary>
        /// This string prevents mangling of type and function names.
        /// This is used in some places to make the auto-dependency system works correctly.
        /// </summary>
        public const string DONT_MANGLE = "_dont_mangle_";
        public const string CONSTANT_TYPE_SUFFIX = "_t";

        public const String XML_G25_SPEC = "g25spec";
        public const String XML_LICENSE = "license";
        public const String XML_GPL = "gpl";
        public const String XML_BSD = "bsd";
        public const String XML_CUSTOM = "custom";
        public const String XML_LANGUAGE = "language";
        public const String XML_C = "c";
        public const String XML_CPP = "cpp";
        public const String XML_JAVA = "java";
        public const String XML_CSHARP = "csharp";
        public const String XML_PYTHON = "python";
        public const String XML_MATLAB = "matlab";
        public const String XML_NAMESPACE = "namespace";
        public const String XML_COORD_STORAGE = "coordStorage";
        public const String XML_ARRAY = "array";
        public const String XML_VARIABLES = "variables";
        public const String XML_DEFAULT_OPERATOR_BINDINGS = "defaultOperatorBindings";
        public const String XML_TRUE = "true";
        public const String XML_FALSE = "false";
        public const String XML_DIMENSION = "dimension";
        public const String XML_REPORT_USAGE = "reportUsage";
        public const String XML_GMV_CODE = "gmvCode";
        public const String XML_EXPAND = "expand";
        public const String XML_RUNTIME = "runtime";
        public const String XML_PARSER = "parser";
        public const String XML_TEST_SUITE = "testSuite";
        public const String XML_NONE = "none";
        public const String XML_ANTLR = "antlr";
        public const String XML_BUILTIN = "builtin";
        public const String XML_CUSTOM_LICENSE = "customLicense";
        public const String XML_INLINE = "inline";
        public const String XML_CONSTRUCTORS = "constructors";
        public const String XML_SET = "set";
        public const String XML_ASSIGN = "assign";
        public const String XML_OPERATORS = "operators";
        public const String XML_FUNCTIONS = "functions";
        public const String XML_FLOAT_TYPE = "floatType";
        public const String XML_TYPE = "type";
        public const String XML_PREFIX = "prefix";
        public const String XML_SUFFIX = "suffix";
        public const String XML_UNARY_OPERATOR = "unaryOperator";
        public const String XML_BINARY_OPERATOR = "binaryOperator";
        public const String XML_FUNCTION = "function";
        public const String XML_SYMBOL = "symbol";
        public const String XML_BASIS_VECTOR_NAMES = "basisVectorNames";
        public const String XML_NAME = "name";
        public const String XML_METRIC = "metric";
        public const String XML_ROUND = "round";
        public const String XML_MV = "mv";
        public const String XML_COMPRESS = "compress";
        public const String XML_BY_GRADE = "byGrade";
        public const String XML_BY_GROUP = "byGroup";
        public const String XML_COORDINATE_ORDER = "coordinateOrder";
        public const String XML_DEFAULT = "default";
        public const String XML_MEM_ALLOC = "memAlloc";
        public const String XML_PARITY_PURE = "parityPure";
        public const String XML_DYNAMIC = "dynamic";
        public const String XML_FULL = "full";
        public const String XML_GROUP = "group";
        public const String XML_SMV = "smv";
        public const String XML_CONST = "const";
        public const String XML_CONSTANT = "constant";
        public const String XML_MULTIVECTOR = "multivector";
        public const String XML_BLADE = "blade";
        public const String XML_ROTOR = "rotor";
        public const String XML_VERSOR = "versor";
        public const String XML_COPYRIGHT = "copyright";
        public const String XML_OUTPUT_NAME = "outputName";
        public const String XML_ARG = "arg";
        public const String XML_OPTION = "option";
        public const String XML_ARGNAME = "argName";
        public const String XML_COMMENT = "comment";
        public const String XML_OM = "om";
        public const String XML_SOM = "som";
        public const String XML_DOMAIN = "domain";
        public const String XML_RANGE = "range";
        public const String XML_RETURN_TYPE = "returnType";
        public const String XML_SCALAR = "scalar";
        public const String XML_OUTPUT_DIRECTORY = "outputDirectory";
        public const String XML_PATH = "path";
        public const String XML_OUTPUT_FILENAME = "outputFilename";
        public const String XML_DEFAULT_NAME = "defaultName";
        public const String XML_CUSTOM_NAME = "customName";
        public const String XML_VERBATIM = "verbatim";
        public const String XML_FILENAME = "filename";
        public const String XML_POSITION = "position";
        public const String XML_MARKER = "marker";
        public const String XML_CODE_FILENAME = "codeFilename";
        public const String XML_TOP = "top";
        public const String XML_BOTTOM = "bottom";
        public const String XML_BEFORE_MARKER = "before";
        public const String XML_AFTER_MARKER = "after";


        /// <summary>
        /// Used as the name of the auto-generated euclidean metric.
        /// </summary>
        public const String INTERNAL_EUCLIDEAN_METRIC = "_internal_euclidean_metric_";


        const String GPL_LICENSE = 
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

        const String BSD_LICENSE = 
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

        /// <summary>
        /// Constructor which sets up a default specification. Can be used to
        /// construct a specification 'by hand' (i.e, using function calls and setting public
        /// members).
        /// </summary>
        public Specification()
        {
            m_inputDirectory = System.IO.Directory.GetCurrentDirectory();

            InitFloatTypes();
            m_copyright = "";
            m_outputLanguage = OUTPUT_LANGUAGE.NONE;
            m_coordStorage = COORD_STORAGE.ARRAY;
            SetDimension(1);
            InitMetric();
            InitBasisBladeParser();

            System.Console.WriteLine("Specification.Specification(): todo: set members to default values");

        }

        /// <summary>
        /// Constructor which reads an XML specification file.
        /// </summary>
        /// <param name="filename">Filename of XML file.</param>
        public Specification(string filename)
        {
            try
            {
                m_inputDirectory = System.IO.Path.GetDirectoryName(filename);
            }
            catch (Exception)
            {
                m_inputDirectory = System.IO.Directory.GetCurrentDirectory();
            }

            InitFloatTypes();
            m_copyright = "";
            m_outputLanguage = OUTPUT_LANGUAGE.NONE;
            InitMetric();
            InitBasisBladeParser();

            SetOutputDir(System.IO.Directory.GetCurrentDirectory());
            m_outputDirectoryExplicitlySet = false; // force to false

            XmlDocument doc = new XmlDocument();
            doc.Load(filename);

            // todo: maybe remember what 'filename' refers to, so we can use it as
            // a relative path for e.g., licenses.

            InitFromXmlDocument(doc);
        }

        /// <summary>
        /// Converts a RefGA.BasisBlade to a string for printout in the specification.
        /// The blade should have positive or negative 1 scale. A scalar basis blade
        /// is transformed into "scalar" or "-scalar".
        /// </summary>
        protected string BasisBladeToString(RefGA.BasisBlade B, String[] bvNames)
        {
            String bbStr = B.ToString(bvNames);
            // convert "-1*" to "-", otherwise the string cannot be parsed back in again
            if (bbStr.StartsWith("-1*")) bbStr = "-" + bbStr.Substring(3);

            if (bbStr == "1") bbStr = "scalar";
            if (bbStr == "-1") bbStr = "-scalar";
            return bbStr;
        }


        /// <summary>
        /// Converts this specification to XML (that can be parsed by the constructor)
        /// </summary>
        /// <returns>An XML string that can be parsed by the constructor</returns>
        public string ToXmlString()
        {
            StringBuilder SB = new StringBuilder();
            SB.Append("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n");
            SB.AppendLine(""); // empty line
            SB.Append("<" + XML_G25_SPEC + "\n");

            bool customLicense = false; // used later on to know whether to emit full license text
            { // output attributes of g25spec element
                { // license
                    String licString = m_license;
                    if (licString == GPL_LICENSE)
                        licString = XML_GPL;
                    else if (licString == BSD_LICENSE)
                        licString = XML_BSD;
                    else
                    {
                        licString = XML_CUSTOM;
                        customLicense = true;
                    }
                    SB.Append("\t" + XML_LICENSE + "=\"" + licString + "\"\n");
                }

                { // copyright
                    if ((m_copyright != null) && (m_copyright.Length > 0))
                        SB.Append("\t" + XML_COPYRIGHT + "=\"" + m_copyright + "\"\n");
                }

                { // language
                    SB.Append("\t" + XML_LANGUAGE + "=\"" + GetOutputLanguageString() + "\"\n");
                }

                // namespace
                if ((m_namespace != null) && (m_namespace.Length > 0))
                    SB.Append("\t" + XML_NAMESPACE + "=\"" + m_namespace + "\"\n");

                // coordinate storage
                SB.Append("\t" + XML_COORD_STORAGE + "=\"" + ((m_coordStorage == COORD_STORAGE.ARRAY) ? XML_ARRAY : XML_VARIABLES) + "\"\n");

                // operator bindings
                SB.Append("\t" + XML_DEFAULT_OPERATOR_BINDINGS + "=\"" + (DefaultOperatorBindings() ? XML_TRUE : XML_FALSE) + "\"\n");

                // dimension
                SB.Append("\t" + XML_DIMENSION + "=\"" + m_dimension.ToString() + "\"\n");

                // report usage of non-optimized functions
                SB.Append("\t" + XML_REPORT_USAGE + "=\"" + (m_reportUsage ? XML_TRUE : XML_FALSE) + "\"\n");

                { // what type of GMV code to generate:
                    SB.Append("\t" + XML_GMV_CODE + "=\"");
                    switch (m_gmvCodeGeneration)
                    {
                        case GMV_CODE.EXPAND: SB.Append(XML_EXPAND); break;
                        case GMV_CODE.RUNTIME: SB.Append(XML_RUNTIME); break;
                        default: SB.Append("BAD GMV CODE OPTION"); break;
                    }
                    SB.Append("\"\n");
                }

                { // what type of parser to generate:
                    SB.Append("\t" + XML_PARSER + "=\"");
                    switch (m_parserType)
                    {
                        case PARSER.NONE: SB.Append(XML_NONE); break;
                        case PARSER.ANTLR: SB.Append(XML_ANTLR); break;
                        case PARSER.BUILTIN: SB.Append(XML_BUILTIN); break;
                        default: SB.Append("BAD PARSER OPTION"); break;
                    }
                    SB.Append("\"\n");
                }

                // generate test suite
                SB.Append("\t" + XML_TEST_SUITE + "=\"" + (m_generateTestSuite ? XML_TRUE : XML_FALSE) + "\"\n");
                
            }

            SB.Append("\t>\n"); // end of <g25spec> entry

            SB.AppendLine(""); // empty line

            if (customLicense) // output custom license
                SB.Append("<" + XML_CUSTOM_LICENSE + ">" + m_license + "</" + XML_CUSTOM_LICENSE + ">\n");

            if (m_verbatimCode.Count > 0) // output verbatim code
            {
                foreach (VerbatimCode VC in m_verbatimCode)
                {
                    // determine if code is in XML or in file:
                    bool hasCode = ((VC.m_verbatimCode != null) && (VC.m_verbatimCode.Length > 0));
                    // open XML tag
                    SB.Append("<" + XML_VERBATIM);

                    // output all filenames to which verbatim code should be applied
                    for (int f = 0; f < VC.m_filenames.Count; f++) {
                       SB.Append(" " + XML_FILENAME);
                       if (f > 0) SB.Append((f+1).ToString());
                       SB.Append("=\"" + VC.m_filenames[f] + "\"");
                    }

                    // output the filename where verbatim code comes from
                    if ((VC.m_verbatimCodeFile != null) && (VC.m_verbatimCodeFile.Length > 0))
                        SB.Append(" " + XML_CODE_FILENAME + "=\"" + VC.m_verbatimCodeFile  + "\"");

                    // output position
                    SB.Append(" " + XML_POSITION + "=");
                    switch (VC.m_where)
                    {
                        case VerbatimCode.POSITION.TOP:
                            SB.Append("\"" + XML_TOP + "\"");
                            break;
                        case VerbatimCode.POSITION.BOTTOM:
                            SB.Append("\"" + XML_BOTTOM + "\"");
                            break;
                        case VerbatimCode.POSITION.BEFORE_MARKER:
                            SB.Append("\"" + XML_BEFORE_MARKER + "\"");
                            break;
                        case VerbatimCode.POSITION.AFTER_MARKER:
                            SB.Append("\"" + XML_AFTER_MARKER + "\"");
                            break;
                    }

                    if ((VC.m_where == VerbatimCode.POSITION.BEFORE_MARKER) ||
                        (VC.m_where == VerbatimCode.POSITION.AFTER_MARKER))
                    {
                        SB.Append(" " + XML_MARKER + "=\"" + VC.m_customMarker + "\"");
                    }

                    // output verbatim code that goes into XML, if present
                    if (hasCode)
                    {
                        SB.Append(">" + VC.m_verbatimCode + "</" + XML_VERBATIM + ">\n");
                    }
                    else SB.Append("/>\n");
                }
            }


            if (m_outputDirectoryExplicitlySet) // output dir
                SB.Append("<" + XML_OUTPUT_DIRECTORY + " " + XML_PATH + "=\"" + m_outputDirectory +"\"/>\n");



            { // overrides
                 foreach( KeyValuePair<string, string> kvp in m_outputFilenameOverrides )
                    {
                        SB.Append("<" + XML_OUTPUT_FILENAME + " " + 
                            XML_DEFAULT_NAME + "=\"" + kvp.Key + "\" " +
                            XML_CUSTOM_NAME + "=\"" + kvp.Value + 
                            "\"/>\n");
                    }
            }

            SB.AppendLine(""); // empty line

            { // inline
                SB.Append("<" + XML_INLINE + "\n");
                SB.Append("\t" + XML_CONSTRUCTORS + "=\"" + ((m_inlineConstructors) ? XML_TRUE : XML_FALSE) + "\"\n");
                SB.Append("\t" + XML_SET + "=\"" + ((m_inlineSet) ? XML_TRUE : XML_FALSE) + "\"\n");
                SB.Append("\t" + XML_ASSIGN + "=\"" + ((m_inlineAssign) ? XML_TRUE : XML_FALSE) + "\"\n");
                SB.Append("\t" + XML_OPERATORS + "=\"" + ((m_inlineOperators) ? XML_TRUE : XML_FALSE) + "\"\n");
                SB.Append("\t" + XML_FUNCTIONS + "=\"" + ((m_inlineFunctions) ? XML_TRUE : XML_FALSE) + "\"\n");
                SB.Append("\t/>\n"); // end of <inline> entry
            }

            SB.AppendLine(""); // empty line

            { // float types
                foreach (FloatType FT in m_floatTypes) {
                    SB.Append("<" + XML_FLOAT_TYPE + " " + XML_TYPE + "=\"" + FT.type + "\"");
                    if (FT.prefix.Length > 0) SB.Append(" " + XML_PREFIX + "=\"" + FT.prefix + "\"");
                    if (FT.suffix.Length > 0) SB.Append(" " + XML_SUFFIX + "=\"" + FT.suffix + "\"");
                    SB.Append("/>\n"); // end of <floatType> entry
                }
            }

            SB.AppendLine(""); // empty line

            { // basis vector names
                SB.Append("<" + XML_BASIS_VECTOR_NAMES);
                for (int i = 0; i < m_basisVectorNames.Count; i++)
                    SB.Append("\n\t" + XML_NAME + (i + 1).ToString() + "=\"" + m_basisVectorNames[i] + "\"");
                SB.Append("\n\t/>\n"); // end of <basisVectorNames> entry
            }

            SB.AppendLine(""); // empty line

            { // metric
                // printed out in order of basisvectors
                foreach (Metric M in m_metric)
                {
                    if (M.m_name == INTERNAL_EUCLIDEAN_METRIC) continue; // do not emit auto-generated metric to XML
                    for (int v1 = 0; v1 < m_dimension; v1++)
                        for (int v2 = 0; v2 < m_dimension; v2++)
                            for (int i = 0; i < M.m_metricBasisVectorIdx1.Count; i++)
                                if ((v1 == M.m_metricBasisVectorIdx1[i]) && (v2 == M.m_metricBasisVectorIdx2[i]))
                                {
                                    SB.Append("<" + XML_METRIC + " " + XML_NAME + "=\"" + M.m_name + "\"");
                                    if (!M.m_round) // default = true, so only print when false
                                        SB.Append(" " + XML_ROUND + "=\"" + XML_FALSE + "\"");
                                    SB.Append(">");
                                    SB.Append(m_basisVectorNames[v1] + "." + m_basisVectorNames[v2] + "=" + M.m_metricValue[i]);
                                    SB.Append("</" + XML_METRIC + ">\n");
                                }
                }
            }

            SB.AppendLine(""); // empty line

            // operators
            foreach (Operator op in m_operators)
            {
                // first check if this isn't a 'default operator'
                if (m_defaultOperators.Contains(op)) continue;

                bool unary = (op.NbArguments == 1);
                string opStr = (unary) ? XML_UNARY_OPERATOR : XML_BINARY_OPERATOR;

                SB.Append("<" + opStr + " " + XML_SYMBOL + "=\"" + op.Symbol + "\" " + XML_FUNCTION + "=\"" + op.FunctionName + "\"");

                if (unary && (!op.IsPrefix)) 
                {
                    SB.Append(" " + XML_PREFIX + "=\"" + XML_FALSE + "\"");
                }

                SB.Append("/>\n");

            }


            SB.AppendLine(""); // empty line

            if (m_GMV != null) // general multivector:
            {
                SB.Append("<" + XML_MV);

                // name
                SB.Append(" " + XML_NAME + "=\"" + m_GMV.Name + "\"");

                // compression (by grade, group)
                bool compressedByGrade = m_GMV.IsGroupedByGrade(m_dimension);
                SB.Append(" " + XML_COMPRESS + "=\"");
                if (compressedByGrade) SB.Append(XML_BY_GRADE + "\"");
                else SB.Append(XML_BY_GROUP + "\"");

                // coordinate order
                bool defaultCoordinateOrder = (compressedByGrade && m_GMV.CompareBasisBladeOrder(rsbbp.ListToDoubleArray(m_basisBladeParser.GetDefaultBasisBlades())));
                SB.Append(" " + XML_COORDINATE_ORDER + "=\"");
                if (defaultCoordinateOrder) SB.Append(XML_DEFAULT + "\"");
                else SB.Append(XML_CUSTOM + "\"");

                // memory allocation method
                SB.Append(" " + XML_MEM_ALLOC + "=\"");
                if (m_GMV.MemoryAllocationMethod == GMV.MEM_ALLOC_METHOD.PARITY_PURE)
                    SB.Append(XML_PARITY_PURE + "\"");
                else if (m_GMV.MemoryAllocationMethod == GMV.MEM_ALLOC_METHOD.FULL)
                    SB.Append(XML_FULL + "\"");
                else SB.Append(XML_DYNAMIC + "\"");
                SB.Append(">\n");

                if (!defaultCoordinateOrder)
                { // emit coordinate order:
                    String[] bvNames = (String[])m_basisVectorNames.ToArray();
                    // loop over all groups:
                    for (int g = 0; g < m_GMV.NbGroups; g++)
                    {
                        SB.Append("<" + XML_GROUP + ">");
                        // loop over all basis blades of group
                        for (int i = 0; i < m_GMV.Group(g).Length; i++)
                        {
                            if (i > 0) SB.Append(" ");

                            String bbStr = BasisBladeToString(m_GMV.BasisBlade(g, i), bvNames);
                            SB.Append(bbStr);
                        }
                        SB.Append("</" + XML_GROUP + ">\n");
                    }
                }

                SB.Append("</" + XML_MV + ">\n");
            }

            SB.AppendLine(""); // empty line

            // specialized multivectors
            for (int i = 0; i < m_SMV.Count; i++)
            {
                SB.Append(SMVtoXmlString(m_SMV[i]));
            }

            SB.AppendLine(""); // empty line

            // constants
            for (int i = 0; i < m_constant.Count; i++)
            {
                // assume only SMV constants for now
                ConstantSMV C = m_constant[i] as ConstantSMV;
                if (C == null) continue;

                // check if type has name X+CONSTANT_TYPE_SUFFIX and is constant
                if ((C.Type.GetName().Equals(C.Name + CONSTANT_TYPE_SUFFIX)) && (C.Type as SMV).IsConstant()) continue;

                SB.Append(ConstantToXmlString(C));
            }

            SB.AppendLine(""); // empty line

            // outermorphisms
            {
                // i = -1 = m_GOM, the rest is m_SOM
                for (int i = -1; i < m_SOM.Count; i++)
                {
                    if (i == 0) SB.AppendLine(""); // empty line

                    OM om = (i == -1) ? m_GOM as OM : m_SOM[i] as OM;
                    if (om == null) continue;
                    string XMLtag = ((om is GOM) ? XML_OM : XML_SOM);

                    SB.Append("<" + XMLtag);

                    // name
                    SB.Append(" " + XML_NAME + "=\"" + om.Name + "\"");

                    // coordinate order:
                    bool rangeEqualsDomain = om.DomainAndRangeAreEqual();
                    bool defaultCoordOrder = rangeEqualsDomain && om.CompareDomainOrder(rsbbp.ListToDoubleArray(m_basisBladeParser.GetDefaultBasisBlades()));
                    SB.Append(" " + XML_COORDINATE_ORDER + "=\"" + ((defaultCoordOrder) ? XML_DEFAULT : XML_CUSTOM) + "\"");

                    // end of XMLtag
                    SB.Append(">\n");

                    if (!defaultCoordOrder)
                    {
                        String[] bvNames = (String[])m_basisVectorNames.ToArray();
                        for (int dr = 0; dr < 2; dr++)
                        {
                            String XML_DOMAIN_OR_RANGE = (dr == 0) ? XML_DOMAIN : XML_RANGE;
                            RefGA.BasisBlade[][] B = (dr == 0) ? om.Domain : om.Range;
                            if ((dr == 1) && rangeEqualsDomain) continue;

                            SB.Append("<" + XML_DOMAIN_OR_RANGE + ">");
                            bool first = true;
                            for (int g = 0; g < B.Length; g++)
                            {
                                for (int b = 0; b < B[g].Length; b++)
                                {
                                    if (!first) SB.Append(" ");

                                    String bbStr = BasisBladeToString(B[g][b], bvNames);
                                    SB.Append(bbStr);

                                    first = false;
                                }
                            }


                            SB.Append("</" + XML_DOMAIN_OR_RANGE + ">\n");
                        
                        }
                        // output domain info

                        if (!rangeEqualsDomain)
                        {
                            // output range info
                        }
                    }


                    SB.Append("</" + XMLtag + ">\n");
                }
            }

            SB.AppendLine(""); // empty line

            // function generation specifications
            for (int i = 0; i < m_functions.Count; i++)
                SB.AppendLine(FunctionToXmlString(m_functions[i]));

            SB.AppendLine(""); // empty line

            SB.Append("</" + XML_G25_SPEC + ">\n");

            return SB.ToString();
        } // end of function ToXmlString()

        /// <summary>
        /// Converts a G25.fgs to an XML string representation.
        /// </summary>
        /// <param name="F"></param>
        /// <returns>XML string representation of 'F'.</returns>
        public string FunctionToXmlString(G25.fgs F)
        {
            StringBuilder SB = new StringBuilder();
            SB.Append("<" + XML_FUNCTION);

            // name
            SB.Append(" " + XML_NAME + "=\"" + F.Name + "\"");

            // output name, if different
            if (F.Name != F.OutputName)
                SB.Append(" " + XML_OUTPUT_NAME + "=\"" + F.OutputName + "\"");

            // return type, if set
            if (F.ReturnTypeName.Length > 0)
                SB.Append(" " + XML_RETURN_TYPE + "=\"" + F.ReturnTypeName + "\"");

            // argument types, names
            for (int a = 0; a < F.NbArguments; a++)
            {
                string argTypeName = F.ArgumentTypeNames[a];

                if (argTypeName.EndsWith(CONSTANT_TYPE_SUFFIX)) // check if it is a constant
                {
                    G25.SMV smv = GetType(argTypeName) as G25.SMV;
                    if ((smv != null) && smv.IsConstant()) { // if constant, remove the "_t" from the typename
                        argTypeName = argTypeName.Substring(0, argTypeName.Length - CONSTANT_TYPE_SUFFIX.Length);
                    }
                }

                SB.Append(" " + XML_ARG + (a + 1).ToString() + "=\"" + F.ArgumentTypeNames[a] + "\"");
                if (F.ArgumentVariableNames[a] != fgs.DefaultArgumentName(a))
                    SB.Append(" " + XML_ARGNAME + (a + 1).ToString() + "=\"" + F.ArgumentVariableNames[a] + "\"");
            }

            // options
            {
                foreach (KeyValuePair<String, String> KVP in F.Options) {
                    SB.Append(" " + XML_OPTION + KVP.Key + "=\"" + KVP.Value + "\"");
                }
            }

            // float names, if not all float names of algebra are used:
            if (!F.UsesAllFloatTypes(m_floatTypes))
            {
                for (int f = 0; f < F.NbFloatNames; f++)
                {
                    SB.Append(" " + XML_FLOAT_TYPE + "=\"" + F.FloatNames[f] + "\"");
                }
            }

            // metric name, if not default
            if (F.MetricName != "default")
                SB.Append(" " + XML_METRIC + "=\"" + F.MetricName + "\"");

            // metric name, if not default
            if (F.Comment.Length > 0)
                SB.Append(" " + XML_COMMENT + "=\"" + F.Comment + "\"");

            SB.Append("/>");

            return SB.ToString();
        } // end of FunctionToXmlString()

        /// <summary>
        /// Converts a G25.SMV to an XML string representation.
        /// </summary>
        /// <param name="smv"></param>
        /// <returns>XML string representation of 'smv'.</returns>
        public String SMVtoXmlString(G25.SMV smv)
        {
            StringBuilder SB = new StringBuilder();
            bool constant = smv.IsConstant() && (GetMatchingConstant(smv) != null);
            string name = smv.Name;

            // remove the extra constant suffix?
            if (constant && name.EndsWith(CONSTANT_TYPE_SUFFIX))
                name = name.Substring(0, name.Length - CONSTANT_TYPE_SUFFIX.Length);

            SB.Append("<" + XML_SMV);

            // name
            SB.Append(" " + XML_NAME + "=\"" + name + "\"");

            // constant?
            if (constant)
                SB.Append(" " + XML_CONST + "=\"" + XML_TRUE + "\"");

            // type
            SB.Append(" " + XML_TYPE + "=\"" + smv.MvTypeString + "\"");

            // end of XML_SMV tag
            SB.Append(">");

            { // emit coordinate order:
                string[] bvNames = (string[])m_basisVectorNames.ToArray();
                // loop over all basis blades
                for (int b = 0; b < smv.Group(0).Length; b++)
                {
                    if (b > 0) SB.Append(" ");
                    string bbStr = BasisBladeToString(smv.BasisBlade(0, b), bvNames);
                    SB.Append(bbStr);

                    // if constant, add '=....'
                    if (smv.IsCoordinateConstant(b))
                        SB.Append("=" + smv.ConstBasisBladeValue(smv.BladeIdxToConstBladeIdx(b)).ToString());
                }
                if (smv.Comment.Length > 0)
                    SB.Append("  <" + XML_COMMENT + ">" + smv.Comment + "</" + XML_COMMENT + ">");
            }

            SB.Append("</" + XML_SMV + ">\n");

            return SB.ToString();
        } // end of SMVtoXmlString()



        /// <summary>
        /// Converts a G25.Constant to an XML string representation.
        /// </summary>
        /// <param name="C"></param>
        /// <returns>XML string representation of 'smv'.</returns>
        public String ConstantToXmlString(G25.Constant C)
        {
            StringBuilder SB = new StringBuilder();

            SB.Append("<" + XML_CONSTANT);

            // name
            SB.Append(" " + XML_NAME + "=\"" + C.Name + "\"");

            // type
            SB.Append(" " + XML_TYPE + "=\"" + C.Type.GetName() + "\"");

            // end of XML_CONSTANT tag
            SB.Append(">");

            { // emit value (assuming only SMV constants, for now)
                SMV smv = C.Type as SMV;
                ConstantSMV Csmv = C as ConstantSMV;
                String[] bvNames = (String[])m_basisVectorNames.ToArray();
                for (int b = 0; b < smv.NbNonConstBasisBlade; b++)
                {
                    RefGA.BasisBlade B = smv.NonConstBasisBlade(b);
                    String basisBladeString = BasisBladeToString(B, bvNames);

                    if (b > 0) SB.Append(" ");
                    SB.Append(basisBladeString);
                    SB.Append("=" + Csmv.Value[b]);
                }
                if (C.Comment.Length > 0)
                    SB.Append("  <" + XML_COMMENT + ">" + C.Comment + "</" + XML_COMMENT + ">");
            }

            SB.Append("</" + XML_CONSTANT + ">\n");

            return SB.ToString();
        } // end of ConstantToXmlString()

        /// <summary>
        /// Initializes an empty Specification from an XML document
        /// </summary>
        private void InitFromXmlDocument(XmlDocument doc)
        {
            XmlElement rootElement = doc.DocumentElement;
            //System.Console.WriteLine(rootElement.Name);
            if (rootElement.Name != XML_G25_SPEC)
                throw new G25.UserException("Missing root element " + XML_G25_SPEC + " in XML file.");

            ParseRootElement(rootElement);

            // initializes the RefGA.Metric variables of the m_metrics
            FinishMetric();

            // check if specification is sane
            CheckSpecificationSanity();
        }

        /// <summary>
        /// Throws exception when specification is not consistent, missing details, etc.
        /// For example, a floating point type must be set.
        /// 
        /// Also patches some minor errors/annoyances, such as a m_floatSuffix being null (sets it to "")
        /// </summary>
        public void CheckSpecificationSanity() {
            // dimension
            if (m_dimension < 1) throw new G25.UserException("Invalid dimension of space " + m_dimension);

            // output language
            if (m_outputLanguage == OUTPUT_LANGUAGE.NONE)
                throw new G25.UserException("No output language set (use XML attribute '" + XML_LANGUAGE + "').");

            // namespace
            if ((m_namespace == null) || (m_namespace.Length < 1))
                throw new G25.UserException("No namespace set (use XML attribute '" + XML_NAMESPACE + "').");

            // float types
            if ((m_floatTypes == null) || (m_floatTypes.Count == 0))
                throw new G25.UserException("No float type set (use XML element '" + XML_FLOAT_TYPE  + "').");
            for (int i = 0; i < m_floatTypes.Count; i++) {
                if ((m_floatTypes[i].type == null) || (m_floatTypes[i].type.Length == 0))
                    throw new Exception("Specification.CheckSpecificationSanity(): empty float type"); // this is an internal error, not a user error
            }

            // basis vector names
            CheckBasisVectorNames();

            // check general multivector
            if (m_GMV == null)
                throw new G25.UserException("Missing general multivector specification (use XML element '" + XML_MV + "').");

            m_GMV.SanityCheck(this, m_basisVectorNames.ToArray());

            // check all SMVs
            foreach (G25.SMV smv in m_SMV)
            {
                smv.SanityCheck(this, m_basisVectorNames.ToArray());
            }

            // check general outermorphism
            if (m_GOM != null)
            {
                if (m_gmvCodeGeneration == GMV_CODE.RUNTIME)
                    throw new Exception("Defining a general outermorphism with 'runtime' code enabled is not supported yet.");
                m_GOM.SanityCheck(m_dimension, m_basisVectorNames.ToArray());
            }

            // check all SOMs
            foreach (G25.SOM som in m_SOM)
            {
                som.SanityCheck(m_dimension, m_basisVectorNames.ToArray());
            }

            // check if metric is diagonal +- 1 when using 
            if (m_gmvCodeGeneration == GMV_CODE.RUNTIME)
            {
                foreach (Metric M in m_metric)
                {
                    if (!M.m_metric.IsSimpleDiagonal())
                        throw new G25.UserException("Only a diagonal metric with -1, 0, +1 values on the diagonal can be used when '" + XML_GMV_CODE + "' is set to '" + XML_RUNTIME + "'.");
                }
            }

            // check function generation specifications
            CheckFGS();
        }

        private void ParseRootElement(XmlElement rootElement)
        {
            ParseRootElementAttributes(rootElement.Attributes);
            
            XmlNode _E = rootElement.FirstChild;
            while (_E != null) 
            {
                XmlElement E = _E as XmlElement;
                if (E != null)
                {
                    switch (E.Name)
                    {
                        case XML_CUSTOM_LICENSE:
                            {
                                if (m_license != XML_CUSTOM)
                                    throw new G25.UserException("License was not set to '" + XML_CUSTOM + "' but there still is a '" + XML_CUSTOM_LICENSE + "' in the specification");
                                XmlText T = E.FirstChild as XmlText;
                                if (T != null)
                                    m_license = T.Value;
                            }
                            break;
                        case XML_OUTPUT_DIRECTORY:
                            SetOutputDir(E.GetAttribute(XML_PATH));
                            break;
                        case XML_OUTPUT_FILENAME:
                            SetOutputFilename(E.GetAttribute(XML_DEFAULT_NAME), E.GetAttribute(XML_CUSTOM_NAME));
                            break;

                        case XML_INLINE:
                            ParseInlineAttributes(E.Attributes);
                            break;
                        case XML_FLOAT_TYPE:
                            ParseFloatTypeAttributes(E.Attributes);
                            break;
                        case XML_UNARY_OPERATOR:
                        case XML_BINARY_OPERATOR:
                            ParseOperatorAttributes(E.Name, E.Attributes);
                            break;
                        case XML_BASIS_VECTOR_NAMES:
                            ParseBasisVectorNamesAttributes(E.Attributes);
                            break;
                        case XML_METRIC:
                            {
                                // name
                                String name = E.GetAttribute(XML_NAME);
                                if (name == null) name = "default";

                                // parse actual metric:
                                XmlText T = E.FirstChild as XmlText;
                                if (T == null) throw new G25.UserException("Invalid  '" + XML_METRIC + "' element in specification.");
                                ParseMetric(name, T.Value);

                                // round?
                                if (E.GetAttribute(XML_ROUND) != null)
                                {
                                    GetMetric(name).m_round = !(E.GetAttribute(XML_ROUND).ToLower() == XML_FALSE);
                                }

                            }
                            break;
                        case XML_MV:
                            ParseMVelementAndAttributes(E);
                            break;
                        case XML_SMV:
                            ParseSMVelementAndAttributes(E);
                            break;
                        case XML_OM:
                            ParseGOMelementAndAttributes(E);
                            break;
                        case XML_SOM:
                            ParseSOMelementAndAttributes(E);
                            break;
                        case XML_CONSTANT:
                            ParseConstantElementAndAttributes(E);
                            break;
                        case XML_FUNCTION:
                            ParseFunction(E);
                            break;
                        case XML_VERBATIM:
                            ParseVerbatim(E);
                            break;
                        default:
                            System.Console.WriteLine("Specification.ParseRootElement(): warning: unknown element '" + E.Name + "' in specification");
                            break;
                    }
                }

                _E = _E.NextSibling;
            } 
            
            
            
        }

        /// <summary>
        /// Parses the attributes of the XML_G25_SPEC root element
        /// </summary>
        private void ParseRootElementAttributes(XmlAttributeCollection A)
        {
            // parse all attributes of the root element
            for (int i = 0; i < A.Count; i++)
            {
                switch (A[i].Name)
                {
                    case XML_LICENSE:
                        SetLicense(A[i].Value);
                        break;
                    case XML_COPYRIGHT:
                        m_copyright = A[i].Value;
                        break;
                    case XML_LANGUAGE:
                        SetLanguage(A[i].Value);
                        break;
                    case XML_NAMESPACE:
                        m_namespace = A[i].Value;
                        break;
                    case XML_COORD_STORAGE:
                        if (A[i].Value == XML_ARRAY)
                            m_coordStorage = COORD_STORAGE.ARRAY;
                        else if (A[i].Value == XML_VARIABLES)
                            m_coordStorage = COORD_STORAGE.VARIABLES;
                        else throw new G25.UserException("XML parsing error: Unknown attribute value '" + A[i].Value + "' for attribute '" + XML_COORD_STORAGE + "'.");
                        break;
                    case XML_DEFAULT_OPERATOR_BINDINGS:
                        if (A[i].Value.ToLower() == XML_TRUE)
                            SetDefaultOperatorBindings();
                        break;
                    case XML_DIMENSION:
                        int dim;
                        try {
                            dim = System.Int32.Parse(A[i].Value);
                        }
                        catch (System.Exception) { throw new G25.UserException("Invalid dimension for space of algebra: '" + A[i].Value + "'."); }
                        SetDimension(dim);
                        break;
                    case XML_REPORT_USAGE:
                        m_reportUsage = (A[i].Value.ToLower() == XML_TRUE);
                        break;
                    case XML_GMV_CODE:
                        if (A[i].Value.ToLower() == XML_RUNTIME)
                            m_gmvCodeGeneration = GMV_CODE.RUNTIME;
                        else if (A[i].Value.ToLower() == XML_EXPAND)
                            m_gmvCodeGeneration = GMV_CODE.EXPAND;
                        else throw new G25.UserException("Invalid value '" + A[i].Value + "' for attribute '" + XML_GMV_CODE + "'.");
                        break;
                    case XML_PARSER:
                        if (A[i].Value.ToLower() == XML_NONE)
                            m_parserType = PARSER.NONE;
                        else if (A[i].Value.ToLower() == XML_ANTLR)
                            m_parserType = PARSER.ANTLR;
                        else if (A[i].Value.ToLower() == XML_BUILTIN)
                            m_parserType = PARSER.BUILTIN;
                        else throw new G25.UserException("Invalid value '" + A[i].Value + "' for attribute '" + XML_PARSER + "'.");
                        break;
                    case XML_TEST_SUITE:
                        m_generateTestSuite = (A[i].Value.ToLower() == XML_TRUE);
                        break;
                    default:
                        throw new G25.UserException("XML parsing error: Unknown XML attribute '" + A[i].Name + "' in root element '" + XML_G25_SPEC + "'.");
                }
            }
        }


        /// <summary>
        /// Parses the attributes of the XML_INLINE element
        /// </summary>
        private void ParseInlineAttributes(XmlAttributeCollection A)
        {
            // parse all attributes of the root element
            for (int i = 0; i < A.Count; i++)
            {
                bool val = (A[i].Value.ToLower() == XML_TRUE);
                switch (A[i].Name)
                {
                    case XML_CONSTRUCTORS:
                        m_inlineConstructors = val;
                        break;
                    case XML_SET:
                        m_inlineSet = val;
                        break;
                    case XML_ASSIGN:
                        m_inlineAssign = val;
                        break;
                    case XML_OPERATORS:
                        m_inlineOperators = val;
                        break;
                    case XML_FUNCTIONS:
                        m_inlineFunctions = val;
                        break;
                    default:
                        throw new G25.UserException("XML parsing error: Unknown attribute '" + A[i].Name + "' in element '" + XML_INLINE + "'.");
                }
             }
        }

        /// <summary>
        /// Parses the attributes of the XML_FLOAT_TYPE element
        /// </summary>
        private void ParseFloatTypeAttributes(XmlAttributeCollection A) 
        {
            String floatType = "", floatSuffix = "", floatPrefix = "";
            for (int i = 0; i < A.Count; i++)
            {
                switch (A[i].Name)
                {
                    case XML_TYPE:
                        floatType = A[i].Value;
                        break;
                    case XML_PREFIX:
                        floatPrefix = A[i].Value;
                        break;
                    case XML_SUFFIX:
                        floatSuffix = A[i].Value;
                        break;
                    default:
                        throw new G25.UserException("XML parsing error: Unknown attribute '" + A[i].Name + "' in element '" + XML_FLOAT_TYPE + "'.");
                }
            }

            AddFloatType(floatType, floatPrefix, floatSuffix);
        }

        private void ParseOperatorAttributes(String elementName, XmlAttributeCollection A) {
            int nbArgs = (elementName == XML_UNARY_OPERATOR) ? 1 : 2;
            String symbol = "";
            String function = "";
            bool prefix = true;
            bool prefixAttributeSpecified = false;

            for (int i = 0; i < A.Count; i++)
            {
                switch (A[i].Name)
                {
                    case XML_PREFIX:
                        prefix = (A[i].Value.ToLower() == XML_TRUE);
                        prefixAttributeSpecified = true;
                        break;
                    case XML_FUNCTION:
                        function = A[i].Value;
                        break;
                    case XML_SYMBOL:
                        symbol = A[i].Value;
                        break;
                    default:
                        throw new G25.UserException("XML parsing error: Unknown attribute '" + A[i].Name + "' in specification.");
                }
            }

            if ((nbArgs != 1) && prefixAttributeSpecified)
                throw new G25.UserException("Prefix specified for operator '" + symbol + "' bound to '" + function + "' (todo: improve this error message).");

            AddOperator(new Operator(nbArgs, prefix, symbol, function));
        }

        /// <summary>
        /// Sets all basis vector names to "", then handle XML attributes (nameX = "..") and then checks if all names have been set.
        /// </summary>
        private void ParseBasisVectorNamesAttributes(XmlAttributeCollection A)
        {
            // reset all names to ""
            for (int i = 0; i < m_basisVectorNames.Count; i++)
                m_basisVectorNames[i] = "";

            // handle all attributes
            for (int i = 0; i < A.Count; i++)
            {
                if (!A[i].Name.StartsWith(XML_NAME))
                    throw new G25.UserException("XML parsing error: Invalid attribute '" + A[i].Name + "' in element '" + XML_BASIS_VECTOR_NAMES + "'.");

                int idx;
                try
                {
                    idx = System.Int32.Parse(A[i].Name.Substring(XML_NAME.Length))-1;
                }
                catch (System.Exception)
                {
                    throw new G25.UserException("XML parsing error: Invalid attribute '" + A[i].Name + "' in element '" + XML_BASIS_VECTOR_NAMES + "'.");
                }
                SetBasisVectorName(idx, A[i].Value);
            }

            // check if all have been set 
            CheckBasisVectorNames();
        }

        /// <summary>
        /// Parses a metric string like "no.ni=-1" and adds it to the metric definitions.
        /// </summary>
        /// <param name="name">name of the metric (e.g., "default" or "euclidean")</param>
        /// <param name="str">A string "no.ni=-1". The string must be of the from id.id=(id.id=)*=(+-)?1. Valid examples are 
        /// "e1.e1=e2.e2=+1.2", "e3.e3=-1.2", 
        /// </param>
        private void ParseMetric(String name, String str)
        {
            Object O = m_metricParser.Parse(str);
            if (O == null) throw new G25.UserException("Error parsing metric specification '" + str + "'.");

            // System.Console.WriteLine("str -> " + O.ToString());
            // assign(e1.e1, assign(e2.e2, assign(e3.e3, 1)))

            G25.rsep.FunctionApplication FA = O as G25.rsep.FunctionApplication;
            if (FA == null) throw new G25.UserException("Invalid metric specification '" + str + "'.");

            ParseMetric(name, FA, str);
        }

        /// <summary>
        /// Called by ParseMetric(String str). Strips the assign()s until it find the value
        /// </summary>
        /// <param name="name">name of the metric (e.g., "default" or "euclidean")</param>
        /// <returns>The value for X . Y metric</returns>
        private double ParseMetric(String name, Object O, String str)
        {
            // first check if 'O' could be the final value of the metric specification
            G25.rsep.FunctionApplication FA = O as G25.rsep.FunctionApplication;
            if ((FA == null) || (FA.FunctionName == "negate") || (FA.FunctionName == "nop")) {
                return ParseMetricValue(O, str);
            }

            // This FA should be of the form X.Y=value
            // First argument must be a XdotY function application, and the function name should be "assign"
            G25.rsep.FunctionApplication XdotY = FA.Arguments[0] as G25.rsep.FunctionApplication;
            if ((FA.NbArguments != 2) || (FA.FunctionName != "assign") || 
                (XdotY == null) || (XdotY.NbArguments != 2) || 
                (XdotY.FunctionName != "ip")) throw new G25.UserException("Invalid metric specification '" + str + "'");

            // get value by recursing
            double value = ParseMetric(name, FA.Arguments[1], str);

            // get, check names of basis vectors
            String basisVectorName1 = XdotY.Arguments[0] as String;
            String basisVectorName2 = XdotY.Arguments[1] as String;
            if ((basisVectorName1 == null) || (basisVectorName2 == null))
                throw new G25.UserException("Invalid basis vector names in metric specification '" + str + "'");

            int basisVectorIdx1 = BasisVectorNameToIndex(basisVectorName1);
            int basisVectorIdx2 = BasisVectorNameToIndex(basisVectorName2);

            SetMetric(name, basisVectorIdx1, basisVectorIdx2, value);

            return value;
        }

        /// <summary>
        /// Called by ParseMetric(Object O, String str). 
        /// </summary>
        /// <param name="O">Either a string or an Object.</param>
        /// <param name="str">The full metric specification (used only for descriptions in Exceptions).</param>
        /// <returns>The value of the metric specification or throws an Exception.</returns>
        private double ParseMetricValue(Object O, String str)
        {
            // is it a string? If so, parse it
            String Ostr = O as String;
            if (Ostr != null)
            {
                try
                {
                    return System.Double.Parse(Ostr);
                }
                catch (System.Exception)
                {
                    throw new G25.UserException("Invalid value in metric specification '" + str + "'");
                }
            }

            // it must be negate(...) or nop(...)
            G25.rsep.FunctionApplication FA = O as G25.rsep.FunctionApplication;
            if ((FA == null) || (FA.NbArguments != 1) || (!((FA.FunctionName == "negate") || (FA.FunctionName == "nop"))))
                throw new G25.UserException("Invalid value in metric specification '" + str + "'");

            if (FA.FunctionName == "negate")
                return -ParseMetricValue(FA.Arguments[0], str);
            else return ParseMetricValue(FA.Arguments[0], str);
        }

        /// <summary>
        /// Parses an XML_MV element and stores the result.
        /// 
        /// The XML_MV element should contain the name, compression method (by grade or by group), 
        /// coordinate order (default or custom) 
        /// and memory allocation method of the general multivector.
        /// </summary>
        /// <param name="E">XML_MV element</param>
        private void ParseMVelementAndAttributes(XmlElement E)
        {
            String name = "mv";
            bool compressByGrade = true; // false means 'by group'
            bool defaultCoordinateOrder = true; // false means 'custom'
            GMV.MEM_ALLOC_METHOD memAllocMethod = GMV.MEM_ALLOC_METHOD.FULL;

            { // handle attributes
                XmlAttributeCollection A = E.Attributes;

                // handle all attributes
                for (int i = 0; i < A.Count; i++)
                {
                    // name
                    if (A[i].Name == XML_NAME)
                        name = A[i].Value;

                    // compress method (grade or group
                    else if (A[i].Name == XML_COMPRESS)
                    {
                        if (A[i].Value == XML_BY_GRADE)
                            compressByGrade = true;
                        else if (A[i].Value == XML_BY_GROUP)
                            compressByGrade = false;
                        else throw new G25.UserException("XML parsing error: Invalid compression '" + A[i].Value + "' in element '" + XML_MV + "'.");
                    }

                    // coordinate order
                    else if (A[i].Name == XML_COORDINATE_ORDER)
                    {
                        if (A[i].Value == XML_DEFAULT)
                            defaultCoordinateOrder = true;
                        else if (A[i].Value == XML_CUSTOM)
                            defaultCoordinateOrder = false;
                        else throw new G25.UserException("XML parsing error: Invalid coordinate order '" + A[i].Value + "' in element '" + XML_MV + "'.");
                    }

                    // memory allocation method
                    else if (A[i].Name == XML_MEM_ALLOC)
                    {
                        if (A[i].Value == XML_PARITY_PURE)
                            memAllocMethod = GMV.MEM_ALLOC_METHOD.PARITY_PURE;
                        else if (A[i].Value == XML_FULL)
                            memAllocMethod = GMV.MEM_ALLOC_METHOD.FULL;
                        else if (A[i].Value == XML_DYNAMIC)
                            memAllocMethod = GMV.MEM_ALLOC_METHOD.DYNAMIC;
                        else throw new G25.UserException("XML parsing error: Invalid memory allocation method '" + A[i].Value + "' in element '" + XML_MV + "'.");
                    }
                }
                
            } // end of 'handle attributes'

            // check for sanity
            if ((compressByGrade == false) && (defaultCoordinateOrder == true)) {
                throw new G25.UserException("Cannot compress by group without a custom coordinate order. Please specify a coordinate order in the '" + XML_MV + "' element.");
            }

            // basis blades go here
            List<List<G25.rsbbp.BasisBlade>> basisBlades = null;

            if (defaultCoordinateOrder)
            {
                // check for sanity
                if (E.FirstChild != null)
                    throw new G25.UserException("The coordinate order is set to default, but the multivector definition element '" + XML_MV + "' contains a custom coordinate order.");

                basisBlades = m_basisBladeParser.GetDefaultBasisBlades();
            }
            else
            {
                basisBlades = m_basisBladeParser.ParseMVbasisBlades(E);
                if (compressByGrade)
                    basisBlades = m_basisBladeParser.SortBasisBladeListByGrade(basisBlades);
            }

            if (rsbbp.ConstantsInList(basisBlades))
                throw new G25.UserException("Constant coordinate(s) were specified in the general multivector type (XML element '" + XML_MV + "')");

            SetGeneralMV(new GMV(name, rsbbp.ListToDoubleArray(basisBlades), memAllocMethod));
        }

        /// <summary>
        /// Parses basis blade list of SMVs and constants.
        /// </summary>
        /// <returns>List of basis blades, or null if 'F' does not contain such a list.</returns>
        private List<G25.rsbbp.BasisBlade> ParseBasisBladeList(XmlNode _F, string parentName)
        {
            while (_F != null)
            {
                XmlText FT = _F as XmlText;

                // is it text?
                if (FT != null)
                {
                    try
                    {
                        return m_basisBladeParser.ParseBasisBlades(FT);
                    }
                    catch (Exception Ex)
                    {
                        throw new G25.UserException("While parsing basis blades of '" + parentName + "':\n" + Ex.Message);
                    }
                }

                _F = _F.NextSibling;
            }
            return null;
        }

        /// <summary>
        /// Parses a comment for SMVs and constants.
        /// </summary>
        /// <returns>Comment string, or null if 'F' does not contain a comment.</returns>
        private string ParseComment(XmlNode _F)
        {
            while (_F != null)
            {
                // or comment?
                XmlElement FE = _F as XmlElement;
                if ((FE != null) && (FE.Name == XML_COMMENT))
                {
                    XmlText CT = FE.FirstChild as XmlText;
                    return CT.Value;
                }

                _F = _F.NextSibling;
            }
            return null;
        }



        /// <summary>
        /// Parses an XML_SMV element and stores the result.
        /// 
        /// The XML_SMV element should contain the name, and optionally the type, const property and constant name.
        /// 
        /// If the const property is true and no name is specified, the regular name is used as the name
        /// of the singleton constant and the name of the type is renamed to name_t.
        /// 
        /// </summary>
        /// <param name="E">XML_SMV element</param>
        private void ParseSMVelementAndAttributes(XmlElement E)
        {
            string typeName = null;
            bool isConstant = false;
            string constantName = null;
            SMV.MULTIVECTOR_TYPE mvType = SMV.MULTIVECTOR_TYPE.MULTIVECTOR;

            { // handle attributes
                XmlAttributeCollection A = E.Attributes;

                // handle all attributes
                for (int i = 0; i < A.Count; i++)
                {
                    // name
                    if (A[i].Name == XML_NAME)
                        typeName = A[i].Value;

                    // const
                    else if (A[i].Name == XML_CONST)
                        isConstant = (A[i].Value.ToLower() == XML_TRUE);

                    // type
                    else if (A[i].Name == XML_TYPE)
                    {
                        if (A[i].Value == XML_MULTIVECTOR)
                            mvType = SMV.MULTIVECTOR_TYPE.MULTIVECTOR;
                        else if (A[i].Value == XML_BLADE)
                            mvType = SMV.MULTIVECTOR_TYPE.BLADE;
                        else if (A[i].Value == XML_ROTOR)
                            mvType = SMV.MULTIVECTOR_TYPE.ROTOR;
                        else if (A[i].Value == XML_VERSOR)
                            mvType = SMV.MULTIVECTOR_TYPE.VERSOR;
                        else throw new G25.UserException("XML parsing error: Invalid value for attribute'" + XML_TYPE + "':'" + A[i].Value + "' in element '" + XML_SMV + "'.");
                    }
                }

                // sanity check on name
                if (typeName == null)
                    throw new G25.UserException("XML parsing error: Missing '" + XML_NAME + "' attribute in element '" + XML_SMV + "'.");

                // if constant and no constantName provided, use the typeName as the constantName, and set typeName to typeName + "_t" 
                if (isConstant && (constantName == null))
                {
                    // if a constant should be generated and no constant name is specified
                    constantName = typeName;
                    typeName = constantName + CONSTANT_TYPE_SUFFIX;
                }

                // check if name is already present
                if (IsTypeName(typeName))
                    throw new G25.UserException("In specialized multivector definition: type '" + typeName + "' already exists.");

            } // end of 'handle attributes'

            // parse list of basis blades and optional comment
            List<G25.rsbbp.BasisBlade> L = ParseBasisBladeList(E.FirstChild, typeName);
            string comment = ParseComment(E.FirstChild);

            if (L == null) 
                throw new G25.UserException("XML parsing error in element '" + XML_SMV + "': Missing basis blade list for specialized multivector '" + typeName + "'");

            SMV smv = new SMV(typeName, L.ToArray(), mvType, comment);

            // add new type to list of specialized multivectors
            AddSpecializedMV(smv);

            // todo: add code for adding constant here
            if (constantName != null)
                AddConstant(new ConstantSMV(constantName, smv, null, comment));
        } // end of ParseSMVelementAndAttributes()

        /// <summary>
        /// Parses an XML_CONSTANT element and stores the result.
        /// 
        /// The XML_CONSTANT element should contain the name, type and basis blades values.
        /// </summary>
        /// <param name="E">XML_CONSTANT element</param>
        private void ParseConstantElementAndAttributes(XmlElement E)
        {
            string constantName = null;
            string typeName = null;

            { // handle attributes
                XmlAttributeCollection A = E.Attributes;

                // handle all attributes
                for (int i = 0; i < A.Count; i++)
                {
                    // name
                    if (A[i].Name == XML_NAME)
                        constantName = A[i].Value;

                    // type
                    else if (A[i].Name == XML_TYPE)
                        typeName = A[i].Value;
                }

                // sanity check on name
                if (constantName == null)
                    throw new G25.UserException("XML parsing error: Missing '" + XML_NAME + "' attribute in element '" + XML_CONSTANT + "'.");
                if (typeName == null)
                    throw new G25.UserException("XML parsing error: Missing '" + XML_TYPE + "' attribute in element '" + XML_CONSTANT + "'.");

                // check if name is already present
                if (!IsSpecializedMultivectorName(typeName))
                    throw new G25.UserException("In constant definition: type '" + typeName + "' is not a specialized multivector.");

            } // end of 'handle attributes'

            // parse list of basis blades and optional comment
            List<G25.rsbbp.BasisBlade> L = ParseBasisBladeList(E.FirstChild, constantName);
            string comment = ParseComment(E.FirstChild);

            SMV type = GetType(typeName) as SMV;

            // add new type to list of specialized multivectors (constuctor should check if all are constant
            AddConstant(new ConstantSMV(constantName, type, L, comment));
        } // end of ParseConstantElementAndAttributes()

        private void ParseGOMelementAndAttributes(XmlElement E) 
        {
            bool specialized = false;
            GOM gom = ParseOMelementAndAttributes(E, specialized) as GOM;
            if (IsTypeName(gom.Name))
                throw new G25.UserException("In general outermorphism definition: a type '" + gom.Name + "' already exists.");
            SetGeneralOM(gom);
        }

                        
        private void ParseSOMelementAndAttributes(XmlElement E) 
        {
            bool specialized = true;
            SOM som = ParseOMelementAndAttributes(E, specialized) as SOM;
            if (IsTypeName(som.Name))
                throw new G25.UserException("In specialized outermorphism definition: a type '" + som.Name + "' already exists.");
            AddSpecializedOM(som);
        }


        /// <summary>
        /// Parses an XML_OM or XML_SOM element and returns the result.
        /// 
        /// The element should contain the name and coordinate order (default or custom) 
        /// of the outermorphism matrix representation.
        /// </summary>
        /// <param name="E">XML_OM or XML_SOM element.</param>
        /// <param name2="specialized">whether a general (false) or specialized (true) outermorphism is being parsed.</param>
        private OM ParseOMelementAndAttributes(XmlElement E, bool specialized)
        {
            String name = null;
            bool defaultCoordinateOrder = false; // false means 'custom'

            { // handle attributes
                XmlAttributeCollection A = E.Attributes;

                // handle all attributes
                for (int i = 0; i < A.Count; i++)
                {
                    // name
                    if (A[i].Name == XML_NAME)
                        name = A[i].Value;

                     // coordinate order
                    else if (A[i].Name == XML_COORDINATE_ORDER)
                    {
                        if (A[i].Value == XML_DEFAULT)
                            defaultCoordinateOrder = true;
                        else if (A[i].Value == XML_CUSTOM)
                            defaultCoordinateOrder = false;
                        else throw new G25.UserException("XML parsing error: Invalid coordinate order '" + A[i].Value + "' in element '" + E.Name + "'.");
                    }
                }
            } // end of 'handle attributes'

            // sanity check name
            if (name == null)
                throw new G25.UserException("XML parsing error: Missing name attribute in element '" + E.Name + "'");

            // domain, range go here 
            List<List<G25.rsbbp.BasisBlade>> domain = null;
            List<List<G25.rsbbp.BasisBlade>> range = null;

            if (defaultCoordinateOrder)
            {
                domain = range = m_basisBladeParser.GetDefaultBasisBlades();
            }
            else
            {
                // get domain & range elements , parse their internals
                XmlNode _DR = E.FirstChild;
                while (_DR != null)
                {
                    XmlElement DR = _DR as XmlElement;
                    switch (DR.Name)
                    {
                        case XML_DOMAIN:
                            domain = m_basisBladeParser.ParseMVbasisBlades(DR);
                            break;
                        case XML_RANGE:
                            range = m_basisBladeParser.ParseMVbasisBlades(DR);
                            break;
                        default:
                            System.Console.WriteLine("XML parsing warning: unknown element '" + E.Name + "' in element '" + E.Name + "'.");
                            break;
                    }
                    _DR = _DR.NextSibling;
                } 
                
            }

            if (domain == null)
                throw new G25.UserException("XML parsing error: Missing element '" + XML_DOMAIN + "' in element '" + E.Name + "' (name=" + name + ").");

            if (rsbbp.ConstantsInList(domain))
                throw new G25.UserException("Constant coordinate(s) in domain of element '" + E.Name + "' (name=" + name + ").");

            if (range == null) range = domain;

            if (specialized)
                return new SOM(name, rsbbp.ListToSingleArray(domain), rsbbp.ListToSingleArray(range), m_dimension);
            else return new GOM(name, rsbbp.ListToSingleArray(domain), rsbbp.ListToSingleArray(range), m_dimension);
        } // end of ParseOMelementAndAttributes()


        public void ParseFunction(XmlElement E)
        {
            // storage for all info:
            String functionName = null;
            String outputFunctionName = null;
            const int MAX_NB_ARGS = 100;
            String returnTypeName = "";
            String[] argumentTypeNames = new String[MAX_NB_ARGS];
            String[] argumentVariableName = new String[MAX_NB_ARGS];
            List<String> floatNames = new List<string>();
            String metricName = "default";
            String comment = "";
            int nbArgs = 0;
            Dictionary<String, String> options = new Dictionary<string, string>();

            { // handle attributes
                XmlAttributeCollection A = E.Attributes;

                // handle all attributes
                for (int i = 0; i < A.Count; i++)
                {
                    // functionName
                    if (A[i].Name == XML_NAME)
                        functionName = A[i].Value;

                    // functionName
                    else if (A[i].Name == XML_OUTPUT_NAME)
                        outputFunctionName = A[i].Value;

                    // metricName
                    else if (A[i].Name == XML_METRIC)
                        metricName = A[i].Value.ToLower();

                    // comment
                    else if (A[i].Name == XML_COMMENT)
                        comment = A[i].Value;

                    // floatType
                    else if (A[i].Name == XML_FLOAT_TYPE)
                        floatNames.Add(A[i].Value);

                     // return type
                    else if (A[i].Name == XML_RETURN_TYPE)
                    {
                        returnTypeName = A[i].Value;
                        if (!IsTypeName(returnTypeName))
                        {
                            if (returnTypeName.ToLower() == XML_SCALAR) // "scalar" is also allowed as returntype
                            {
                                returnTypeName = XML_SCALAR;
                            }
                            else throw new G25.UserException("Error parsing function '" + functionName + "': '" + returnTypeName + "' is not a type (inside element '" + XML_FUNCTION + "').");
                        }
                    }

                    // argNameX
                    else if (A[i].Name.StartsWith(XML_ARGNAME))
                    {
                        int argNameIdx = 0;
                        try
                        {
                            argNameIdx = System.Int32.Parse(A[i].Name.Substring(XML_ARGNAME.Length)) - 1;
                        }
                        catch (System.Exception)
                        {
                            throw new G25.UserException("Error parsing function '" + functionName + "': invalid attribute '" + A[i].Name + "' in element '" + XML_FUNCTION + "'.");
                        }
                        if ((argNameIdx >= argumentVariableName.Length) || (argNameIdx < 0))
                            throw new G25.UserException("Error parsing function '" + functionName + "': invalid attribute index '" + A[i].Name + "' in element '" + XML_FUNCTION + "'.");

                        argumentVariableName[argNameIdx] = A[i].Value;
                    }

                    // argX
                    else if (A[i].Name.StartsWith(XML_ARG))
                    {
                        int argIdx = 0;
                        try
                        {
                            argIdx = System.Int32.Parse(A[i].Name.Substring(XML_ARG.Length)) - 1;
                        }
                        catch (System.Exception)
                        {
                            throw new G25.UserException("Error parsing function '" + functionName + "': invalid attribute '" + A[i].Name + "' in element '" + XML_FUNCTION + "'.");
                        }
                        if ((argIdx >= argumentTypeNames.Length) || (argIdx < 0))
                            throw new G25.UserException("Error parsing function '" + functionName + "': invalid attribute index '" + A[i].Name + "' in element '" + XML_FUNCTION + "'.");

                        string typeName = A[i].Value;
                        if (!IsTypeName(typeName))
                        {
                            // it may be a constant, like 'e1', try adding a "_t"
                            if (IsTypeName(typeName + CONSTANT_TYPE_SUFFIX))
                            {
                                typeName = typeName + CONSTANT_TYPE_SUFFIX;
                            }
                            else throw new G25.UserException("Error parsing function '" + functionName + "': '" + typeName + "' is not a type (inside element '" + XML_FUNCTION + "')");
                        }

                        argumentTypeNames[argIdx] = typeName;
                        if (argIdx >= nbArgs)
                            nbArgs = argIdx + 1;
                    }

                    else if (A[i].Name.StartsWith(XML_OPTION))
                    {
                        String optionName = A[i].Name.Substring(XML_OPTION.Length).ToLower();
                        if (optionName.Length > 0) {
                            String optionValue = A[i].Value;
                            options[optionName] = optionValue;
                        }
                    }
                }

                // check if function name was specified:
                if (functionName == null)
                    throw new G25.UserException("Missing attribute '" + XML_NAME + "' in element '" + XML_FUNCTION + "'");

                // if output function name is missing, just use the regular function name
                if (outputFunctionName == null) outputFunctionName = functionName;

                // if no float type are specified, copy all from specification
                if (floatNames.Count == 0)
                {
                    foreach (FloatType FT in m_floatTypes)
                        floatNames.Add(FT.type);
                }

                // resize arguments arrays:
                Array.Resize(ref argumentTypeNames, nbArgs);
                Array.Resize(ref argumentVariableName, nbArgs);

                // check for nulls in argument arrays
                for (int i = 0; i < argumentTypeNames.Length; i++)
                {
                    if (argumentTypeNames[i] == null)
                        throw new G25.UserException("XML parsing error in function '" + functionName  + "': Missing attribute '" + XML_ARG + (1 + i).ToString() + "' in element '" + XML_FUNCTION + "'");
                    if (argumentVariableName[i] == null)
                        argumentVariableName[i] = fgs.DefaultArgumentName(i);                        
                }
            }

            fgs F = new fgs(functionName, outputFunctionName, returnTypeName, argumentTypeNames, argumentVariableName, floatNames.ToArray(), metricName, comment, options);

            m_functions.Add(F);
        } // ParseFunction()

        /// <summary>
        /// Checks all functions. Throws an Exception when an error is found.
        /// Currently, only the existance of the metric is checked.
        /// </summary>
        private void CheckFGS()
        {
            foreach (fgs F in m_functions)
            {
                if (!IsMetric(F.MetricName))
                    throw new G25.UserException("Unknown metric " + F.MetricName,
                        FunctionToXmlString(F));
            }
        }

        /// <summary>
        /// Finds and returns the function (fgs) that matches the description.
        /// Returns null if not found.
        /// </summary>
        /// <param name="functionName">Name (as 'recognized' by the code generator) of the function.</param>
        /// <param name="argumentTypes">Argument types (can be null or empty for default args).</param>
        /// <param name="floatName">Floating point types to generate for (can be a subset of the returned function).</param>
        /// <param name="metricName">Name of metric used in function (can be null if you don't care about the metric).</param>
        /// <returns>The function generation specification, or null if no matching FGS found.</returns>
        public G25.fgs FindFunctionEx(String functionName, String[] argumentTypes, String floatName, String metricName)
        {
            return FindFunctionEx(functionName, argumentTypes, new String[] { floatName }, metricName);
        }

        /// <summary>
        /// Finds and returns the function (fgs) that matches the description.
        /// Returns null if not found.
        /// </summary>
        /// <param name="functionName">Name (as 'recognized' by the code generator) of the function.</param>
        /// <param name="argumentTypes">Argument types (can be null or empty for default args).</param>
        /// <param name="floatNames">Floating point types to generate for (can be a subset of the returned function).</param>
        /// <param name="metricName">Name of metric used in function (can be null if you don't care about the metric).</param>
        /// <returns>The function generation specification, or null if no matching FGS found.</returns>
        public G25.fgs FindFunctionEx(string functionName, string[] argumentTypes, string[] floatNames, string metricName)
        {
            String returnTypeName = null;
            return FindFunctionEx(functionName, argumentTypes, returnTypeName, floatNames, metricName);
        }

        /// <summary>
        /// Finds and returns the function (fgs) that matches the description.
        /// Returns null if not found. Pointers for argument types are not compared??
        /// </summary>
        /// <param name="functionName">Name (as 'recognized' by the code generator) of the function.</param>
        /// <param name="argumentTypes">Argument types (can be null or empty for default args).</param>
        /// <param name="returnTypeName">Name of the return type (can be null or "" for default return type).</param>
        /// <param name="floatNames">Floating point types to generate for (can be a subset of the returned function).</param>
        /// <param name="metricName">Name of metric used in function (can be null if you don't care about the metric).</param>
        /// <returns>The function generation specification, or null if no matching FGS found.</returns>
        public G25.fgs FindFunctionEx(string functionName, string[] argumentTypes, string returnTypeName, string[] floatNames, string metricName)
        {
            foreach (fgs F in m_functions) 
            {
                // check name
                if (F.Name != functionName) continue;

                // check argument types
                if ((argumentTypes != null) && (argumentTypes.Length > 0)) 
                {
                    if (argumentTypes.Length != F.NbArguments) continue;
                    bool match = true;
                    for (int a = 0; a < argumentTypes.Length; a++) 
                    {
                        if (F.ArgumentTypeNames[a] != argumentTypes[a])
                        {
                            match = false;
                            break;
                        }
                    }
                    if (match == false) continue;
                }

                // find all float names
                bool allFloatTypesFound = true;
                for (int f = 0; f < floatNames.Length; f++) {
                    bool found = false;
                    for (int g = 0; g < F.FloatNames.Length; g++)
                        if (F.FloatNames[g] == floatNames[f])
                            found = true;
                    if (!found)
                    {
                        allFloatTypesFound = false;
                        break;
                    }
                }
                if (!allFloatTypesFound) continue;

                // check metric
                if (metricName != null)
                    if (metricName != F.MetricName) continue;

                if ((returnTypeName != null) && (returnTypeName.Length > 0))
                {
                    //??? TODO!!!! How do we know the default . . . . 
                    if (F.ReturnTypeName != returnTypeName) continue;
                }
                
                return F;
            }
            return null;
        } // end of FindFunction()


        public void ParseVerbatim(XmlElement E)
        {
            List<string> filenames = new List<string>();
            VerbatimCode.POSITION where = VerbatimCode.POSITION.INVALID;
            string customMarker = null;
            string verbatimCode = null;
            string verbatimCodeFile = null;

            { // handle attributes
                XmlAttributeCollection A = E.Attributes;

                // handle all attributes
                for (int i = 0; i < A.Count; i++)
                {
                    // filename
                    if (A[i].Name.StartsWith(XML_FILENAME)) {
                       filenames.Add(A[i].Value);
                    }

                    // position
                    else if (A[i].Name == XML_POSITION) {
                        if (A[i].Value == XML_TOP) where = VerbatimCode.POSITION.TOP;
                        else if (A[i].Value == XML_BOTTOM) where = VerbatimCode.POSITION.BOTTOM;
                        else if (A[i].Value == XML_BEFORE_MARKER) where = VerbatimCode.POSITION.BEFORE_MARKER;
                        else if (A[i].Value == XML_AFTER_MARKER) where = VerbatimCode.POSITION.AFTER_MARKER;
                        else throw new G25.UserException("Invalid " + XML_POSITION + " '" + A[i].Value + "'  in element '" + XML_VERBATIM + "'.");
                    }

                    // marker
                    else if (A[i].Name == XML_MARKER) {
                        customMarker = A[i].Value;
                    }

                    // codeFilename
                    else if (A[i].Name == XML_CODE_FILENAME) {
                        if (A[i].Value.Length > 0)
                            verbatimCodeFile = A[i].Value;
                    }

                    else throw new G25.UserException("Invalid attribute '" + A[i].Name + "'  in element '" + XML_VERBATIM + "'.");
                }

                { // get verbatim code from _inside_ the element:
                    XmlText T = E.FirstChild as XmlText;
                    if ((T != null) && (T.Length > 0)) verbatimCode = T.Value;
                }

                // check if function name was specified:
                if (filenames.Count == 0)
                    throw new G25.UserException("Missing attribute '" + XML_FILENAME + "' in element '" + XML_VERBATIM + "'");

                if (where == VerbatimCode.POSITION.INVALID)
                    throw new G25.UserException("Missing attribute '" + XML_POSITION + "' in element '" + XML_VERBATIM + "'");

                if (((where == VerbatimCode.POSITION.BEFORE_MARKER) || 
                    (where == VerbatimCode.POSITION.AFTER_MARKER)) && 
                    (customMarker == null)) {
                        throw new G25.UserException("Missing attribute '" + XML_MARKER + "' in element '" + XML_VERBATIM + "'");                    
                }

                if ((verbatimCode == null) && (verbatimCodeFile == null))
                    throw new G25.UserException("Missing/empty verbatim code or verbatim code filename in element '" + XML_VERBATIM + "'");                    
            } // end of 'handle attributes'

            m_verbatimCode.Add(new VerbatimCode(filenames, where, customMarker, verbatimCode, verbatimCodeFile));
        } // end of ParseVerbatim()



        private void InitBasisBladeParser()
        {
            m_basisBladeParser = new rsbbp(this);
        }

        /// <summary>
        /// Called by constructors; allocates empty lists for float types
        /// </summary>
        private void InitFloatTypes()
        {
            if (m_floatTypes == null) m_floatTypes = new List<FloatType>();
        }

        /// <summary>
        /// Called by constructors; allocates empty arrays for metric info.
        /// Also initializes parser for metric 
        /// </summary>
        private void InitMetric()
        {
            // init list of metrics and add default metric
            m_metric = new List<Metric>();
            m_metric.Add(new Metric("default"));

            { // initialize parser
                bool UNARY = true, BINARY = false;
                bool PREFIX = false;
                bool LEFT_ASSOCIATIVE = true, RIGHT_ASSOCIATIVE = false;
                G25.rsep.Operator[] ops = new G25.rsep.Operator[] {
                    // symbol, name, precedence, unary, postfix, left associative
                    new G25.rsep.Operator("-", "negate", 0, UNARY, PREFIX, LEFT_ASSOCIATIVE),
                    new G25.rsep.Operator("+", "nop", 0, UNARY, PREFIX, LEFT_ASSOCIATIVE), // unary + is a nop
                    new G25.rsep.Operator(".", "ip", 1, BINARY, PREFIX, LEFT_ASSOCIATIVE),
                    new G25.rsep.Operator("=", "assign", 2, BINARY, PREFIX, RIGHT_ASSOCIATIVE) 
                };
                m_metricParser = new G25.rsep(ops);
            }
        }

        /// <summary>
        /// Initializes all RefGA.Metric variables inside each m_metric.
        /// 
        /// If there is not Euclidean metric, adds one, named "_internal_euclidean_". 
        /// This system generated metric will not be written back to XML.
        /// </summary>
        private void FinishMetric()
        {
            bool hasEucl = false;
            foreach (Metric M in m_metric)
            {
                M.Init(m_dimension);
                hasEucl |= M.m_metric.IsEuclidean();
            }

            if (!hasEucl)
            {
                Metric M = null;
                for (int i = 0; i < m_dimension; i++)
                    M = SetMetric(INTERNAL_EUCLIDEAN_METRIC, i, i, 1.0);
                if (M != null) M.Init(m_dimension);
            }
        }

        /// <returns>true when 'metricName' is an existing metric</returns>
        public bool IsMetric(String metricName)
        {
            metricName = metricName.ToLower();
            {
                foreach (Metric M in m_metric)
                {
                    if (M.m_name == metricName)
                        return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Return the metric with name 'metricName'. Creates it if it does not already exist.
        /// Case insensitive.
        /// </summary>
        /// <param name="metricName">Name of the metric (e.g., "default", or "euclidean"). </param>
        /// <returns>requested metric.</returns>
        public Metric GetMetric(String metricName)
        {
            metricName = metricName.ToLower();
            {
                foreach (Metric M in m_metric)
                {
                    if (M.m_name == metricName)
                        return M;
                }
            }
            {
                Metric M = new Metric(metricName);
                m_metric.Add(M);
                return M;
            }
        }

        /// <summary>
        /// Return a Euclidean metric. Returns null if it does not exist (this would be internal error, since it should always exist
        /// after the spec has been initialized.
        /// Case insensitive.
        /// </summary>
        /// <returns>euclidean metric.</returns>
        public Metric GetEuclideanMetric()
        {
            foreach (Metric M in m_metric)
            {
                if (M.m_metric.IsEuclidean())
                    return M;
            }
            return null;
        }

        /// <summary>
        /// Sets basisVectorIdx1.basisVectorIdx2=value.
        /// Overrides existing definitions. The order of basisVectorIdx1 and basisVectorIdx2 doesn't matter.
        /// </summary>
        /// <param name="metricName">Name of the metric (e.g., "default", or "euclidean"). </param>
        /// <param name="basisVectorIdx1">Index of first basis vector.</param>
        /// <param name="basisVectorIdx2">Index of second basis vector.</param>
        /// <param name="value">the value of (basisVectorIdx1.basisVectorIdx2)</param>
        /// <returns>the metric which was set.</returns>
        public Metric SetMetric(String metricName, int basisVectorIdx1, int basisVectorIdx2, double value)
        {
            Metric M = GetMetric(metricName);

            // make sure first index is smallest one:
            if (basisVectorIdx2 < basisVectorIdx1)
            {
                int tmp = basisVectorIdx2;
                basisVectorIdx2 = basisVectorIdx1;
                basisVectorIdx2 = tmp;
            }
                
            // check if already exists (both ways); if so: overwrite
            for (int i = 0; i < M.m_metricBasisVectorIdx1.Count; i++)
            {
                if ((M.m_metricBasisVectorIdx1[i] == basisVectorIdx1) &&
                    (M.m_metricBasisVectorIdx2[i] == basisVectorIdx2))
                {
                    // this metric was already defined; override it:
                    M.m_metricValue[i] = value;
                    return M;
                }
            }

            // not defined yet; simply add new metric
            M.m_metricBasisVectorIdx1.Add(basisVectorIdx1);
            M.m_metricBasisVectorIdx2.Add(basisVectorIdx2);
            M.m_metricValue.Add(value);

            return M;
        }



        /// <summary>
        /// Adds a floating point type to the specification.
        /// </summary>
        /// <param name="floatType">Name of float type (e.g. "float" or "double")</param>
        /// <param name="floatPrefix">Prefix for multivector types using this float (e.g. "" or "float_")</param>
        /// <param name="floatSuffix">Suffix for multivector types using this float (e.g. "" or "_f")</param>
        public void AddFloatType(String floatType, String floatPrefix, String floatSuffix)
        {
            if (floatType.Length == 0) throw new Exception("Specification.AddFloatType(): empty floating point type"); // internal error?

            if (floatPrefix == null) floatPrefix = "";
            if (floatSuffix == null) floatSuffix = "";

            // check if prefix, suffix is unique:
            for (int i = 0; i < m_floatTypes.Count; i++)
            {
                if ((m_floatTypes[i].prefix == floatPrefix) && (m_floatTypes[i].suffix == floatSuffix))
                    throw new G25.UserException("While adding a new floating point type '" + floatType + "': a floating point type '" + m_floatTypes[i] + "'with the same suffix and prefix already exists.");
            }

            m_floatTypes.Add(new FloatType(floatType, floatPrefix, floatSuffix));
        }

        /// <summary>
        /// Searches through m_floatTypes for the type that has the name 'floatType'.
        /// Returns it.
        /// </summary>
        /// <param name="floatType">The name of the float type.</param>
        /// <returns>The G25.FloatType with the name 'floatType', or null if not found.</returns>
        public FloatType GetFloatType(String floatType)
        {
            foreach (FloatType FT in m_floatTypes) {
                if (FT.type == floatType) return FT;
            }
            return null;
        }

        /// <summary>
        /// Adds an operator binding. If the operator was already in use, the old binding
        /// is overwritten.
        /// </summary>
        public void AddOperator(Operator newOp)
        {
            if ((newOp.Symbol == null) || (newOp.Symbol.Length == 0))
                throw new Exception("Specification.AddOperator(): empty operator symbol"); // internal error?
            if ((newOp.FunctionName == null) || (newOp.FunctionName.Length == 0))
                throw new Exception("Specification.AddOperator(): empty operator function"); // internal error?
            if ((newOp.NbArguments < 1) || (newOp.NbArguments > 2))
                throw new Exception("Specification.AddOperator(): invalid number of arguments for operator '" + newOp.Symbol + "' bound to '" + newOp.FunctionName + "' (" + newOp.NbArguments + ")"); // internal error?

            int pos = -1;
            // check if operator is already in used
            for (int i = 0; i < m_operators.Count; i++)
            {
                Operator otherOp = m_operators[i];
                bool prefixMatch = false;
                if ((newOp.NbArguments == 1) && (otherOp.IsPrefix == newOp.IsPrefix)) prefixMatch = true;

                if ((otherOp.NbArguments == newOp.NbArguments) && (otherOp.Symbol == newOp.Symbol) && prefixMatch) 
                    pos = i;
            }

            // if not already defined, create new position, otherwise overwrite the existing operator
            if (pos < 0) {
                pos = m_operators.Count;
                m_operators.Add(null);
            }

            m_operators[pos] = newOp;
        }

        /// <returns>true if 'typeName' is a floating point type, general multivector, specialized multivector, general outermorphism or specialized outermorphism name.</returns>
        public bool IsTypeName(String typeName)
        {
            
            return (IsFloatType(typeName) || 
                (m_GMV != null) && (m_GMV.Name == typeName)) ||
                IsSpecializedMultivectorName(typeName) ||
                ((m_GOM != null) && (m_GOM.Name == typeName)) ||
                IsSpecializedOutermorphismName(typeName) ||
                IsFloatType(typeName);
        }

        /// <summary>
        /// Returns the type according to a typeName.
        /// </summary>
        /// <param name="typeName">(non-mangled) name of type. For example "float" or "mv".</param>
        /// <returns>type specified by 'typeName', or null if not found.</returns>
        public G25.VariableType GetType(string typeName)
        {
            // float?
            G25.VariableType VT = GetFloatType(typeName);
            if (VT != null) return VT;
            // smv?
            VT = GetSMV(typeName);
            if (VT != null) return VT;
            // som?
            VT = GetSOM(typeName);
            if (VT != null) return VT;
            // gmv?
            if ((m_GMV != null) && (m_GMV.Name == typeName)) return m_GMV;
            // gom?
            if ((m_GOM != null) && (m_GOM.Name == typeName)) return m_GOM;

            if (typeName.Equals("CoordinateOrder"))
                return new G25.EnumType("CoordinateOrder");

            return null;
        }

        /// <returns>true if 'typeName' is a specialized multivector name.</returns>
        public bool IsSpecializedMultivectorName(String typeName)
        {
            for (int i = 0; i < m_SMV.Count; i++)
            {
                if (m_SMV[i].Name == typeName) return true;
            }
            return false;
        }

        /// <returns>true if 'typeName' is a general or specialized multivector name.</returns>
        public bool IsMultivectorName(String typeName)
        {
            return (m_GMV.Name == typeName) || 
                IsSpecializedMultivectorName(typeName);
        }

        /// <returns>specialized multivector with name 'typeName', or null if none found.</returns>
        public G25.SMV GetSMV(String typeName)
        {
            for (int i = 0; i < m_SMV.Count; i++)
            {
                if (m_SMV[i].Name == typeName) return m_SMV[i];
            }
            return null;
        }

        /// <returns>specialized multivector with name 'typeName', or null if none found.</returns>
        public G25.SOM GetSOM(String typeName)
        {
            for (int i = 0; i < m_SOM.Count; i++)
            {
                if (m_SOM[i].Name == typeName) return m_SOM[i];
            }
            return null;
        }

        /// <returns>true if 'typeName' is a specialized multivector name.</returns>
        public bool IsSpecializedOutermorphismName(String typeName)
        {
            for (int i = 0; i < m_SOM.Count; i++)
            {
                if (m_SOM[i].Name == typeName) return true;
            }
            return false;
        }

        /// <returns>true if 'typeName' is a general or specialized outermorphism name.</returns>
        public bool IsOutermorphismName(String typeName)
        {
            return ((m_GOM != null) && (m_GOM.Name == typeName)) ||
                IsSpecializedOutermorphismName(typeName);
        }


        /// <returns>true if 'typeName' is a floating point type listed in m_floatTypes.</returns>
        public bool IsFloatType(String typeName)
        {
            foreach (FloatType FT in m_floatTypes)
                if (FT.type == typeName) return true;
            return false;
        }

        /// <summary>
        /// Returns the non-constant scalar SMV, if any. Otherwise return null.
        /// </summary>
        /// <returns>The scalar SMV type, or null if it was not defined.</returns>
        public G25.SMV GetScalarSMV()
        {
            foreach (G25.SMV smv in m_SMV) 
            {
                if ((smv.NbNonConstBasisBlade == 1) && (smv.Group(0)[0].Grade() == 0))
                    return smv;
            }
            return null;
        }

        /// <summary>
        /// Sets license to 'license'. If 'license' is "gpl" (XML_GPL), then the license is set to the full
        /// GPL license. If 'license' is "bsd" (XML_BSD), then the full BSD license is set. Otherwise the
        /// license is simply set to the value of 'license'.
        /// </summary>
        /// <param name="license"></param>
        public void SetLicense(String license)
        {
            if (license.ToLower() == XML_GPL)
            {
                m_license = GPL_LICENSE;
            }
            else if (license.ToLower() == XML_BSD)
            {
                m_license = BSD_LICENSE;
            }
            else if (license.ToLower() == XML_CUSTOM)
            {
                m_license = XML_CUSTOM; // the license will be filled in by the XML_CUSTOM_LICENSE later on
            }
            else throw new G25.UserException("Unknown license '" + license + "' specified.");
        }

        /// <returns>license text (may be multiple lines).</returns>
        public String GetLicense() {
            return m_license;
        }

        /// <summary>
        /// Sets language of emitted code.
        /// </summary>
        /// <param name="language">valid values are "cpp", "java" and "csharp".
        /// (XML_C, XML_CPP, XML_JAVA, XML_CSHARP, XML_PYTHON, XML_MATLAB)
        /// </param>
        public void SetLanguage(String language)
        {
            switch (language.ToLower()) {
                case XML_C: m_outputLanguage = OUTPUT_LANGUAGE.C;
                    break;
                case XML_CPP: m_outputLanguage = OUTPUT_LANGUAGE.CPP;
                    break;
                case XML_CSHARP: m_outputLanguage = OUTPUT_LANGUAGE.CSHARP;
                    break;
                case XML_JAVA: m_outputLanguage = OUTPUT_LANGUAGE.JAVA;
                    break;
                case XML_MATLAB: m_outputLanguage = OUTPUT_LANGUAGE.MATLAB;
                    break;
                case XML_PYTHON: m_outputLanguage = OUTPUT_LANGUAGE.PYTHON;
                    break;
                default:
                    throw new G25.UserException("Unknown output language " + language);
            }
        }

        /// <returns>true if operator bindings as defined are the default for the output language.</returns>
        public bool DefaultOperatorBindings()
        {
            return m_defaultOperators.Count != 0;
        }

        /// <summary>
        /// (re)sets the operator bindings to the default for the current language.
        /// </summary>
        /// <remarks>The list <c>m_defaultOperators</c> must be set with the default operators, because this is
        /// used by the ToXmlString() function to determine whether the default operators were set.</remarks>
        public void SetDefaultOperatorBindings()
        {
            if (m_outputLanguage == OUTPUT_LANGUAGE.NONE)
            {
                throw new G25.UserException("No output language has been set (use XML attribute '" + XML_LANGUAGE + "').");
            }
            else if (m_outputLanguage == OUTPUT_LANGUAGE.C)
            {
                System.Console.WriteLine("Warning: No operator bindings are possible for output language C");
            }
            else if (m_outputLanguage == OUTPUT_LANGUAGE.CPP)
            {
                SetDefaultOperatorBindingsCpp();
            }
            else System.Console.WriteLine("Internal error: Specification.SetDefaultOperatorBindings(): todo: implement this function !");
        }

        /// <summary>
        /// Set the default operator bindings for C++.
        /// </summary>
        private void SetDefaultOperatorBindingsCpp()
        {
            AddOperator(Operator.Binary("+", "add"));
            AddOperator(Operator.Binary("-", "subtract"));
            AddOperator(Operator.UnaryPrefix("-", "negate"));

            AddOperator(Operator.Binary("%", "sp"));
            AddOperator(Operator.Binary("<<", "lc"));
            AddOperator(Operator.Binary(">>", "rc"));
            AddOperator(Operator.Binary("^", "op"));
            AddOperator(Operator.Binary("*", "gp"));
            AddOperator(Operator.Binary("/", "igp"));

            AddOperator(Operator.Binary("&", "meet"));
            AddOperator(Operator.Binary("|", "join"));

            AddOperator(Operator.UnaryPrefix("++", "increment"));
            AddOperator(Operator.UnaryPostfix("++", "increment"));
            AddOperator(Operator.UnaryPrefix("--", "decrement"));
            AddOperator(Operator.UnaryPostfix("--", "decrement"));

            AddOperator(Operator.UnaryPrefix("*", "dual"));
            AddOperator(Operator.UnaryPrefix("!", "versorInverse"));
            AddOperator(Operator.UnaryPrefix("~", "reverse"));

            // remember the default operators (this is used by ToXmlString())
            m_defaultOperators = new List<Operator>(m_operators);
        }

        /// <summary>
        /// Sets the dimension of the algebra.
        /// Also set basis vector to default names (e1, e2, ...)
        /// </summary>
        /// <param name="dim">The dimension of space. Must be >= 1</param>
        public void SetDimension(int dim)
        {
            if (dim < 1) throw new G25.UserException("Invalid dimension for space: " + dim + " (attribute '" + XML_DIMENSION + "').");

            m_dimension = dim;
            if (m_basisVectorNames == null) m_basisVectorNames = new List<string>();
            else m_basisVectorNames.Clear();
            for (int i = 0; i < dim; i++)
            {
                m_basisVectorNames.Add("e" + (i + 1).ToString());
            }
        }

        
        /// <summary>
        /// Searches for basis vector with name 'name', returns its index.
        /// </summary>
        /// <param name="name">Name of the basis vector (e.g. <c>"e1"</c>).</param>
        /// <returns>Index of basis vector, or -1 if not found.</returns>
        public int GetBasisVectorIndex(String name) 
        {
            for (int i = 0; i < m_basisVectorNames.Count; i++)
                if (m_basisVectorNames[i] == name) return i;
            return -1;
        }

        /// <summary>
        /// Sets the general multivector type (checks if name is not already in use, throws if it is.).
        /// </summary>
        public void SetGeneralMV(GMV gmv)
        {
            if (IsTypeName(gmv.Name))
                throw new G25.UserException("While setting the general multivector type: the name '" + gmv.Name + "' is already a typename (XML element '" + XML_MV + "'.");
            m_GMV = gmv;
        }

        /// <summary>
        /// Adds a new specialized multivector (checks if name is not already in use, throws if it is.).
        /// </summary>
        public void AddSpecializedMV(SMV smv)
        {
            if (IsTypeName(smv.Name))
                throw new G25.UserException("While adding a specialized multivector type: the name '" + smv.Name + "' is already a typename.");
            else m_SMV.Add(smv);
        }

        /// <summary>
        /// Sets the general outermorphism type (checks if name is not already in use, throws if it is.).
        /// </summary>
        public void SetGeneralOM(GOM gom)
        {
            if (IsTypeName(gom.Name))
                throw new Exception("While setting the general outermorphism type: the name '" + gom.Name + "' is already a typename (XML element '" + XML_OM + "'.");
            m_GOM = gom;
        }

        /// <summary>
        /// Adds a new specialized outermorphism (checks if name is not already in use, throws if it is.).
        /// </summary>
        public void AddSpecializedOM(SOM som)
        {
            if (IsTypeName(som.Name))
                throw new G25.UserException("While adding a specialized outermorphism type: the name '" + som.Name + "' is already a typename (XML element '" + XML_SOM + "'.");
            else m_SOM.Add(som);
        }

        /// <summary>
        /// Adds a new constant to the specification.
        /// </summary>
        public void AddConstant(Constant C)
        {
            if (IsConstant(C.Name))
                throw new G25.UserException("While adding a specialized multivector type: the name '" + C.Name + "' is already a constant.");
            m_constant.Add(C);
        }

        /// <returns>Constant with name 'name', or null if no such constant.</returns>
        public Constant GetConstant(string name)
        {
            foreach (Constant C in m_constant)
            {
                if (C.Name.Equals(name))
                    return C;
            }
            return null;
        }

        /// <summary>
        /// Returns true when 'name' is a constant.
        /// </summary>
        public bool IsConstant(string name)
        {
            foreach (Constant C in m_constant)
            {
                if (C.Name.Equals(name))
                    return true;
            }
            return false;
        }

        public Constant GetMatchingConstant(VariableType T)
        {
            if (!T.GetName().EndsWith(CONSTANT_TYPE_SUFFIX)) return null;
            string constantName = T.GetName().Substring(0, T.GetName().Length - CONSTANT_TYPE_SUFFIX.Length);

            Constant C = GetConstant(constantName);

            if (C.Type == T) return C;
            else return null;
        }


        /// <summary>
        /// Checks if all basis vector names are unique and valid. Throws Exception when this is
        /// not that case.
        /// </summary>
        private void CheckBasisVectorNames()
        {
            if (m_basisVectorNames.Count != m_dimension)
                throw new G25.UserException("The number of basis vector names does not match dimension of space (see XML element '" + XML_BASIS_VECTOR_NAMES + "').");

            for (int i = 0; i < m_basisVectorNames.Count; i++)
                if ((m_basisVectorNames[i] == null) ||
                    (m_basisVectorNames[i].Length == 0))
                    throw new G25.UserException("Missing (null) or empty basis vector name (see XML element '" + XML_BASIS_VECTOR_NAMES + "').");

            for (int i = 0; i < m_basisVectorNames.Count; i++)
                for (int j = i+1; j < m_basisVectorNames.Count; j++)
                    if (m_basisVectorNames[i] == m_basisVectorNames[j])
                        throw new G25.UserException("Identical basis vector names (" + m_basisVectorNames[i] + "), see XML element '" + XML_BASIS_VECTOR_NAMES + "'.");
        }

        /// <summary>
        /// Sets basis vector name; throws exception when name is invalid or already in use
        /// </summary>
        /// <param name="idx">Index of name (int range [0, m_dimension-1]</param>
        /// <param name="name">name of basis vector</param>
        public void SetBasisVectorName(int idx, String name)
        {
            if ((name == null) || (name.Length == 0))
                throw new G25.UserException("Empty basis vector name (see XML element '" + XML_BASIS_VECTOR_NAMES + "').");
            if ((idx < 0) || (idx >= m_basisVectorNames.Count))
                throw new G25.UserException("Basis vector index (" + idx + ") for basisvector '" + name + "' out of range (see XML element '" + XML_BASIS_VECTOR_NAMES + "').");
            for (int i = 0; i < m_basisVectorNames.Count; i++)
                if (m_basisVectorNames[i] == name)
                    throw new G25.UserException("Duplicate basis vector names (" + name + "), see XML element '" + XML_BASIS_VECTOR_NAMES + "'.");

            m_basisVectorNames[idx] = name;
        }

        /// <param name="name">The name of a basis vector</param>
        /// <returns>index (in range [0, dim) ) of basis vector with name 'name', or throws an Exception when basis vector 'name' is not defined.</returns>
        public int BasisVectorNameToIndex(String name)
        {
            for (int i = 0; i < m_basisVectorNames.Count; i++)
                if (m_basisVectorNames[i] == name)
                    return i;
            throw new Exception("Invalid basis vector name: '" + name +"'");
        }

        /// <summary>
        /// Sets the output directory to 'dirName'.
        /// Also sets 'm_outputDirectoryExplicitlySet' to true (used for XML output to know whether to emit the dir to XML)
        /// </summary>
        public void SetOutputDir(String dirName)
        {
            m_outputDirectory = dirName;
            m_outputDirectoryExplicitlySet = true;
        }

        /// <returns>the directory name for file output.</returns>
        public String GetOutputDir()
        {
            return m_outputDirectory;
        }

        /// <summary>
        /// Sets an override from 'defaultName' to 'customName'.
        /// </summary>
        /// <param name="defaultName">Default </param>
        /// <param name="customName"></param>
        public void SetOutputFilename(String defaultName, String customName)
        {
            m_outputFilenameOverrides[defaultName] = customName;
        }

        /// <returns>output filename of file 'defaultName', with possible override.</returns>
        public String GetOutputFilename(String defaultName)
        {
            if (m_outputFilenameOverrides.ContainsKey(defaultName))
                return m_outputFilenameOverrides[defaultName];
            else return defaultName;
        }

        /// <remarks>First defaultName is possibly overriden using GetOutputFilename().
        /// Then if the filename is rooted (absolute) it is returned as is. Otherwise 
        /// GetOutputDir() is appended in front of it.</remarks>
        /// <returns>Full path of output filename 'defaultName'.</returns>
        public String GetOutputPath(String defaultName)
        {
            String outputFilename = GetOutputFilename(defaultName);
            if (System.IO.Path.IsPathRooted(outputFilename)) return outputFilename;
            else return System.IO.Path.Combine(GetOutputDir(), outputFilename);
        }

        /// <returns>string representation of m_outputLanguage.</returns>
        public String GetOutputLanguageString() {
            if (m_outputLanguage == OUTPUT_LANGUAGE.C) return XML_C;
            else if (m_outputLanguage == OUTPUT_LANGUAGE.CPP) return XML_CPP;
            else if (m_outputLanguage == OUTPUT_LANGUAGE.JAVA) return XML_JAVA;
            else if (m_outputLanguage == OUTPUT_LANGUAGE.CSHARP) return XML_CSHARP;
            else if (m_outputLanguage == OUTPUT_LANGUAGE.PYTHON) return XML_PYTHON;
            else if (m_outputLanguage == OUTPUT_LANGUAGE.MATLAB) return XML_MATLAB;
            return "invalid";
        }

        /// <summary>
        /// Returns the appropriate inline string for the output language.
        /// 
        /// Returns "" when inline is false, and the inline string + postFixStr otherwise.
        /// </summary>
        /// <param name="inline"></param>
        /// <param name="postFixStr">Concatenated to inline string. May be null.</param>
        /// <returns>Inline string, or ""</returns>
        public String GetInlineString(bool inline, String postFixStr)
        {
            if (inline)
            {
                if (this.m_outputLanguage == OUTPUT_LANGUAGE.C)
                    return "";
                else if (this.m_outputLanguage == OUTPUT_LANGUAGE.CPP)
                    return "inline" + postFixStr;
                else return "inline_str_to_do" + postFixStr;
            }
            else return "";
        }

        /**
         * Inserts the verbatim code (in m_verbatimCode</c>) into the generated files.
         * The list <c>generatedFiles</c> is used to find the names of the files.
         * 
         * Warnings are issued when code could not be inserted.
         * */
        public void InsertVerbatimCode(List<string> generatedFiles)
        {
            foreach (VerbatimCode VC in m_verbatimCode) {
                VC.InsertCode(m_inputDirectory, generatedFiles);
            }
        }




        /// <summary>
        ///  The copyright of the generated code.
        /// </summary>
        public String m_copyright;

        /// <summary>
        ///  The license of the generated code.
        /// </summary>
        public String m_license;

        /// <summary>
        /// The namespace for the generated code (can be empty string for no namespace).
        /// </summary>
        public String m_namespace;

        /// <summary>
        /// The language if the generated implementation?
        /// </summary>
        public OUTPUT_LANGUAGE m_outputLanguage;

        /// <summary>
        /// If m_coordStorage is ARRAY, coordinates of specialized multivectors will be stored in arrays.
        /// If m_coordStorage is VARIABLES, coordinates of specialized multivectors will be stored seperate variables.
        /// </summary>
        public COORD_STORAGE m_coordStorage;

        /// <summary>
        /// The dimension of the generated space.
        /// </summary>
        public int m_dimension;

        /// <summary>
        /// Whether to report usage of non-optimized functions (adds printfs to the generated code).
        /// </summary>
        public bool m_reportUsage;

        /// <summary>
        /// What type of code to generate for general multivector functions
        /// (fully expand, or do stuff at run-time to save code size)
        /// </summary>
        public GMV_CODE m_gmvCodeGeneration = GMV_CODE.EXPAND;

        /// <summary>
        /// What type of parser to generate: none, custom (hand-written)
        /// or ANTLR (resulting grammar needs to be compiled and linked to ANTLR runtime library).
        /// </summary>
        public PARSER m_parserType = PARSER.NONE;

        /// <summary>
        /// When true, testing code will be generated.
        /// </summary>
        public bool m_generateTestSuite = false;



        /// <summary>Whether to inline the constructors.</summary>
        public bool m_inlineConstructors;
        /// <summary>Whether to inline the 'set' functions.</summary>
        public bool m_inlineSet;
        /// <summary>Whether to inline the assignment functions.</summary>
        public bool m_inlineAssign;
        /// <summary>Whether to inline the operators.</summary>
        public bool m_inlineOperators;
        /// <summary>Whether to inline the regular functions (like geometric product).</summary>
        public bool m_inlineFunctions;

        /// <summary>The floating point type(s) of the generated code.</summary>
        public List<FloatType> m_floatTypes;

        /// <summary>Whether to add the default operator bindings on init.</summary>
        public bool m_defaultOperatorBindings = false;

        /// <summary>Operator bindings.</summary>
        public List<Operator> m_operators = new List<Operator>();

        /// <summary>Default operator bindings (used to known which operators in <c>m_operators</c>are used-defined and which are default).</summary>
        private List<Operator> m_defaultOperators = new List<Operator>();

        /// <summary>Names of basis vectors (e.g., "e1", "e2", ...)</summary>
        public List<String> m_basisVectorNames;

        /// <summary>
        /// List of Metric objects. The first entry is always the default algebra metric.
        /// </summary>
        public List<Metric> m_metric;

        /// <summary>Used to parse metric specifications (like "no.ni=-1")</summary>
        private G25.rsep m_metricParser;

        /// <summary>Used to parse list of basis blades</summary>
        private G25.rsbbp m_basisBladeParser;

        /// <summary>General multivector specification.</summary>
        public GMV m_GMV;

        /// <summary>Specialized multivector specifications.</summary>
        public List<SMV> m_SMV = new List<SMV>();

        /// <summary>General outermorphism matrix representation specification.
        /// Can be null if the user does not want any GOM.</summary>
        public GOM m_GOM;

        /// <summary>Specialized outermorphism matrix representation specifications.</summary>
        public List<SOM> m_SOM = new List<SOM>();

        /// <summary>Constants</summary>
        public List<Constant> m_constant = new List<Constant>();

        /// <summary>List of functions (G25.fgs) to instantiate.</summary>
        public List<fgs> m_functions = new List<fgs>();

        /// <summary>Where the specification came from, or when this is unknown, the current working directory.
        /// This directory is used to resolve relative paths (e.g., for verbatim code to be read from user-specified files).
        /// </summary>
        public String m_inputDirectory;

        /// <summary>Where the generated files go by default.</summary>
        public String m_outputDirectory;

        /// <summary>Is used to determine whether the output directory should be written to XML</summary>
        public bool m_outputDirectoryExplicitlySet;

        /// <summary>
        /// Overrides for filenames. A map from default names to custom, user specified names.
        /// </summary>
        public Dictionary<String, String> m_outputFilenameOverrides = new Dictionary<String, String>();

        /// <summary>
        /// Verbatim code fragments that should be inserted into output files.
        /// </summary>
        public List<VerbatimCode> m_verbatimCode = new List<VerbatimCode>();

    }


} // end of namespace G25
