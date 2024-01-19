﻿using DynamicSQL.Compiler;
using Microsoft.Data.SqlClient;

var statement = StatementCompiler.Compile<QueryInput>(
    i =>
        $"""
         SELECT
             p.Name,
             << {i.IncludeAddresses} ? (SELECT a.Name FROM Address a WHERE a.PersonId = p.Id FOR JSON AUTO) : '' >> AS Addresses
             FROM Person p
             WHERE 1=1
                 << {i.BirthDate} ? AND p.BirthDate = {i.BirthDate} >>
                 << {i.PeopleIds} ? AND p.Id IN {i.PeopleIds} >>
         """);

var input = new QueryInput(
    null,
    new[] { 1, 2, 3, 4 },
    true);

await using var connection = new SqlConnection("...");

var result = await statement.QueryListAsync<QueryResult>(connection, input);


public record QueryResult(string Name, string Addresses);

public record QueryInput(DateOnly? BirthDate, IEnumerable<int> PeopleIds, bool IncludeAddresses);
