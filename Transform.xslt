<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<!-- Define parameters for filtering -->
	<xsl:param name="name" select="''" />
	<xsl:param name="faculty" select="''" />
	<xsl:param name="department" select="''" />
	<xsl:param name="subjects" select="''" />
	<xsl:param name="marks" select="''" />

	<!-- Define parameters for column visibility -->
	<xsl:param name="showName" select="true()" />
	<xsl:param name="showFaculty" select="true()" />
	<xsl:param name="showDepartment" select="true()" />
	<xsl:param name="showSubjects" select="true()" />
	<xsl:param name="showMarks" select="true()" />

	<xsl:template match="/">
		<html>
			<head>
				<style>
					table { border-collapse: collapse; width: 100%; }
					th, td { border: 1px solid black; padding: 8px; text-align: left; }
					th { background-color: #f2f2f2; }
				</style>
			</head>
			<body>
				<table>
					<tr>
						<xsl:if test="$showName='true'">
							<th>Name</th>
						</xsl:if>
						<xsl:if test="$showFaculty='true'">
							<th>Faculty</th>
						</xsl:if>
						<xsl:if test="$showDepartment='true'">
							<th>Department</th>
						</xsl:if>
						<xsl:if test="$showSubjects='true'">
							<th>Subjects</th>
						</xsl:if>
						<xsl:if test="$showMarks='true'">
							<th>Marks</th>
						</xsl:if>
					</tr>

					<xsl:for-each select="//publication[
                        ($name='' or @NAME=$name) and
                        ($faculty='' or @FACULTY=$faculty) and
                        ($department='' or @DEPARTMENT=$department) and
                        ($subjects='' or @SUBJECTS=$subjects) and
                        ($marks='' or @MARKS=$marks) 
                    ]">
						<tr>
							<xsl:if test="$showName='true'">
								<td>
									<xsl:value-of select="@NAME"/>
								</td>
							</xsl:if>
							<xsl:if test="$showFaculty='true'">
								<td>
									<xsl:value-of select="@FACULTY"/>
								</td>
							</xsl:if>
							<xsl:if test="$showDepartment='true'">
								<td>
									<xsl:value-of select="@DEPARTMENT"/>
								</td>
							</xsl:if>
							<xsl:if test="$showSubjects='true'">
								<td>
									<xsl:value-of select="@SUBJECTS"/>
								</td>
							</xsl:if>
							<xsl:if test="$showMarks='true'">
								<td>
									<xsl:value-of select="@MARKS"/>
								</td>
							</xsl:if>
						</tr>
					</xsl:for-each>
				</table>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>